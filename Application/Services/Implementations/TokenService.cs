using Data.Entities;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides token creation services, including JWT access tokens and long-lived refresh tokens.
    /// This class is documented by AI.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="config">
        /// Application configuration used to resolve JWT settings such as issuer, audience,
        /// signing key, access token lifetime, and refresh token lifetime.
        /// </param>
        public TokenService(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// Creates a signed JWT access token for the specified user.
        /// </summary>
        /// <param name="user">The authenticated <see cref="User"/> for whom the token is issued.</param>
        /// <param name="extraClaims">
        /// Optional additional claims to include in the token payload (e.g., custom permissions).
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>accessToken</c>: the compact JWT string.</description></item>
        /// <item><description><c>expiresUtc</c>: the UTC expiration date for the token.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The following configuration keys are expected under the <c>Jwt</c> section:
        /// <c>Issuer</c>, <c>Audience</c>, <c>SigningKey</c>, and <c>AccessTokenMinutes</c>.
        /// </remarks>
        public (string accessToken, DateTime expiresUtc) CreateAccessToken(User user, IEnumerable<Claim>? extraClaims = null)
        {
            var jwt = config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenMinutes"]!));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("ss", user.SecurityStamp),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (extraClaims != null) claims.AddRange(extraClaims);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        /// <summary>
        /// Creates a new refresh token tied to a specific user and session.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="sessionId">The unique identifier of the user's active session.</param>
        /// <returns>
        /// A newly generated <see cref="RefreshToken"/> containing a secure random token and UTC expiry.
        /// </returns>
        /// <remarks>
        /// Uses a cryptographically strong random generator (64 bytes) and sets expiration based on
        /// <c>Jwt:RefreshTokenDays</c> from configuration.
        /// </remarks>
        public RefreshToken CreateRefreshToken(int userId, int sessionId)
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(bytes);
            var days = int.Parse(config["Jwt:RefreshTokenDays"]!);

            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                SessionId = sessionId,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(days)
            };
        }
    }
}
