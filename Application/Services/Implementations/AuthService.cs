using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.Auth;
using Application.Exceptions;
using Application.Mappers;
using Data.Entities;
using Domain.Validations;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides authentication flows such as sign-up, sign-in, token refresh, and sign-out.
    /// Coordinates user/session persistence and token issuance.
    /// This class is documented by AI.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthService"/>.
        /// </summary>
        /// <param name="userRepository">Repository for user data access.</param>
        /// <param name="sessionRepository">Repository for session tracking.</param>
        /// <param name="refreshTokenRepository">Repository for refresh token storage and queries.</param>
        /// <param name="unitOfWork">Unit of Work to persist and coordinate transactions.</param>
        /// <param name="passwordHasher">Password hasher used to verify and create password hashes.</param>
        /// <param name="tokenService">Service responsible for issuing access and refresh tokens.</param>
        public AuthService(
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Exchanges a valid refresh token for a new access token and a new refresh token.
        /// </summary>
        /// <param name="dto">The request containing the refresh token to exchange.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="SignInResponse"/> containing a new access token, its expiration,
        /// and the newly issued refresh token.
        /// </returns>
        /// <exception cref="BadRequestException">Thrown when the refresh token is missing.</exception>
        /// <exception cref="UnauthorizedException">
        /// Thrown when the refresh token is invalid, expired, or the user is inactive.
        /// </exception>
        public async Task<SignInResponse> RefreshToken(RefreshTokenRequest dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                throw new BadRequestException(AuthValidator.RefreshTokenRequiredErrorMessage);

            var existing = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, ct);
            if (existing == null || !existing.IsActive)
                throw new UnauthorizedException(AuthValidator.InvalidOrExpiredRefreshTokenErrorMessage);

            var user = await _userRepository.GetByIdAsync(ct, existing.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedException(AuthValidator.InactiveUserErrorMessage);

            // revoke the old refresh token and chain to the new one
            existing.RevokedAtUtc = DateTime.UtcNow;
            var newRt = _tokenService.CreateRefreshToken(existing.UserId, existing.SessionId);
            existing.ReplacedByToken = newRt.Token;
            _refreshTokenRepository.Add(newRt);

            var (access, exp) = _tokenService.CreateAccessToken(user);

            await _unitOfWork.SaveChangesAsync(ct);

            return AuthMapper.ToResponse(access, exp, newRt, user, existing.SessionId);
        }

        /// <summary>
        /// Authenticates a user with credentials, starts a new session, and issues tokens.
        /// </summary>
        /// <param name="dto">The sign-in credentials (email/username and password).</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="SignInResponse"/> containing the access token, its expiration,
        /// the refresh token, and session details.
        /// </returns>
        /// <exception cref="UnauthorizedException">
        /// Thrown when credentials are invalid or the user is inactive.
        /// </exception>
        public async Task<SignInResponse> SignIn(SignInRequest dto, CancellationToken ct = default)
        {
            var user = await _userRepository.GetAsync(
                u => u.Email == dto.EmailOrUsername || u.Username == dto.EmailOrUsername,
                asTracking: true,
                ct: ct);

            if (user == null)
                throw new UnauthorizedException(AuthValidator.InvalidCredentialsErrorMessage);

            var pwd = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (pwd == PasswordVerificationResult.Failed)
                throw new UnauthorizedException(AuthValidator.InvalidCredentialsErrorMessage);

            if (!user.IsActive)
                throw new UnauthorizedException(AuthValidator.InactiveUserErrorMessage);

            // create session
            var session = new Session { UserId = user.Id };
            _sessionRepository.Add(session);
            await _unitOfWork.SaveChangesAsync(ct);

            // optional custom claims example
            var extraClaims = new[] { new Claim("perm", "products.read") };
            var (access, exp) = _tokenService.CreateAccessToken(user, extraClaims);

            // issue refresh token
            var rt = _tokenService.CreateRefreshToken(user.Id, session.Id);
            _refreshTokenRepository.Add(rt);

            await _unitOfWork.SaveChangesAsync(ct);

            return AuthMapper.ToResponse(access, exp, rt, user, session);
        }

        /// <summary>
        /// Signs out by revoking tokens and deactivating sessions.
        /// </summary>
        /// <param name="dto">
        /// The sign-out request. If <c>AllSessions</c> is true, all active sessions for the current user are closed;
        /// otherwise, a specific refresh token is required.
        /// </param>
        /// <param name="currentUserId">The ID of the currently authenticated user initiating the sign-out.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="BadRequestException">
        /// Thrown when a specific sign-out is requested but the refresh token is missing.
        /// </exception>
        /// <exception cref="NotFoundException">
        /// Thrown when the provided refresh token is not found or does not belong to the user.
        /// </exception>
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

            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                throw new BadRequestException(AuthValidator.RefreshTokenRequiredWhenAllSessionsFalseErrorMessage);

            var rt = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken, ct);
            if (rt == null || rt.UserId != currentUserId)
                throw new NotFoundException(AuthValidator.RefreshTokenNotFoundErrorMessage);

            // revoke the specific refresh token and close its session
            rt.RevokedAtUtc = DateTime.UtcNow;

            var session = await _sessionRepository.GetByIdAsync(ct, rt.SessionId);
            if (session != null && session.IsActive)
            {
                session.IsActive = false;
                session.LogoutAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Registers a new user with the provided data and stores a hashed password.
        /// </summary>
        /// <param name="dto">The sign-up data containing email, username, and password.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="SignUpResponse"/> describing the newly created user.</returns>
        /// <exception cref="ConflictException">
        /// Thrown when the email or username is already taken.
        /// </exception>
        public async Task<SignUpResponse> SignUp(SignUpRequest dto, CancellationToken ct = default)
        {
            if (await _userRepository.ExistsAsync(u => u.Email == dto.Email || u.Username == dto.Username, ct))
                throw new ConflictException(AuthValidator.EmailOrUsernameAlreadyTakenErrorMessage);

            var user = AuthMapper.ToEntity(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return AuthMapper.ToResponse(user);
        }
    }
}
