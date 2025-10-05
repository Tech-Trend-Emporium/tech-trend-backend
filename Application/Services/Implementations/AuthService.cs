using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.Auth;
using Application.Exceptions;
using Data.Entities;
using Domain.Entities;
using Domain.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ISessionRepository sessionRepository, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<SignInResponse> RefreshToken(RefreshTokenRequest dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken)) throw new BadRequestException(AuthValidator.RefreshTokenRequiredErrorMessage);

            var existing = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, ct);
            if (existing == null || !existing.IsActive) throw new UnauthorizedException(AuthValidator.InvalidOrExpiredRefreshTokenErrorMessage);

            var user = await _userRepository.GetByIdAsync(ct, existing.UserId);
            if (user == null || !user.IsActive) throw new UnauthorizedException(AuthValidator.InactiveUserErrorMessage);

            existing.RevokedAtUtc = DateTime.UtcNow;
            var newRt = _tokenService.CreateRefreshToken(existing.UserId, existing.SessionId);
            existing.ReplacedByToken = newRt.Token;
            _refreshTokenRepository.Add(newRt);

            var (access, exp) = _tokenService.CreateAccessToken(user);

            await _unitOfWork.SaveChangesAsync(ct);

            return new SignInResponse
            {
                AccessToken = access,
                AccessTokenExpiresAtUtc = exp,
                RefreshToken = newRt.Token,
                RefreshTokenExpiresAtUtc = newRt.ExpiresAtUtc,
                Role = user.Role.ToString(),
                SessionId = existing.SessionId
            };
        }

        public async Task<SignInResponse> SignIn(SignInRequest dto, CancellationToken ct = default)
        {
            var user = await _userRepository.GetAsync(u => u.Email == dto.EmailOrUsername || u.Username == dto.EmailOrUsername, asTracking: true, ct: ct);
            if (user == null) throw new UnauthorizedException(AuthValidator.InvalidCredentialsErrorMessage);

            var pwd = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (pwd == PasswordVerificationResult.Failed) throw new UnauthorizedException(AuthValidator.InvalidCredentialsErrorMessage);

            if (!user.IsActive) throw new UnauthorizedException(AuthValidator.InactiveUserErrorMessage);

            var session = new Session { UserId = user.Id };
            _sessionRepository.Add(session);
            await _unitOfWork.SaveChangesAsync(ct);

            var extraClaims = new[] { new Claim("perm", "products.read") };
            var (access, exp) = _tokenService.CreateAccessToken(user, extraClaims);

            var rt = _tokenService.CreateRefreshToken(user.Id, session.Id);
            _refreshTokenRepository.Add(rt);

            await _unitOfWork.SaveChangesAsync(ct);

            return new SignInResponse
            {
                AccessToken = access,
                AccessTokenExpiresAtUtc = exp,
                RefreshToken = rt.Token,
                RefreshTokenExpiresAtUtc = rt.ExpiresAtUtc,
                Role = user.Role.ToString(),
                SessionId = session.Id
            };
        }

        public async Task SignOut(SignOutRequest dto, int currentUserId, CancellationToken ct = default)
        {
            if (dto.AllSessions)
            {
                var activeSessions = await _sessionRepository.GetActiveByUserAsync(currentUserId, ct);
                foreach (var s in activeSessions)
                {
                    s.IsActive = false;
                    s.LogoutAt = DateTime.UtcNow;
                }

                await _refreshTokenRepository.RevokeAllActiveByUserAsync(currentUserId, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                return;
            }

            if (string.IsNullOrWhiteSpace(dto.RefreshToken)) throw new BadRequestException(AuthValidator.RefreshTokenRequiredWhenAllSessionsFalseErrorMessage);

            var rt = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, ct);
            if (rt == null || rt.UserId != currentUserId) throw new NotFoundException(AuthValidator.RefreshTokenNotFoundErrorMessage);

            rt.RevokedAtUtc = DateTime.UtcNow;

            var session = await _sessionRepository.GetByIdAsync(ct, rt.SessionId);
            if (session != null && session.IsActive)
            {
                session.IsActive = false;
                session.LogoutAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<SignUpResponse> SignUp(SignUpRequest dto, CancellationToken ct = default)
        {
            if (await _userRepository.ExistsAsync(u => u.Email == dto.Email || u.Username == dto.Username, ct)) throw new ConflictException(AuthValidator.EmailOrUsernameAlreadyTakenErrorMessage);

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Role = Domain.Enums.Role.SHOPPER,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return new SignUpResponse {
                Id = user.Id, 
                Email = user.Email, 
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}