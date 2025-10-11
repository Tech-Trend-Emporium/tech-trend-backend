using Application.Dtos.Auth;
using Data.Entities;
using Domain.Entities;
using General.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class AuthMapper
    {
        public static SignUpResponse ToResponse(User entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new SignUpResponse
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.Username,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static SignInResponse ToResponse(string accessToken, DateTime expiresAt, RefreshToken refreshToken, User user, Session session)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            if (expiresAt == default) throw new ArgumentNullException(nameof(expiresAt));
            if (refreshToken is null) throw new ArgumentNullException(nameof(refreshToken));
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (session is null) throw new ArgumentNullException(nameof(session));

            return new SignInResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = expiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                Role = user.Role.ToString(),
                SessionId = session.Id
            };
        }

        public static SignInResponse ToResponse(string accessToken, DateTime expiresAt, RefreshToken refreshToken, User user, int sessionId)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentNullException(nameof(accessToken));
            if (expiresAt == default) throw new ArgumentNullException(nameof(expiresAt));
            if (refreshToken is null) throw new ArgumentNullException(nameof(refreshToken));
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));

            return new SignInResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = expiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                Role = user.Role.ToString(),
                SessionId = sessionId
            };
        }
    }
}
