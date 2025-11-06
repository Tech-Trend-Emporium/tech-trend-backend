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

        /// <summary>
        /// Generates a short-lived JSON Web Token (JWT) used specifically for password reset operations.
        /// The token embeds the user's unique identifier, email address, security stamp, and a dedicated
        /// <c>purpose</c> claim to differentiate it from standard access tokens.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> entity for whom the password reset token will be created.
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item>
        /// <description><c>token</c>: The generated password reset JWT string.</description>
        /// </item>
        /// <item>
        /// <description><c>expiresUtc</c>: The UTC expiration time of the token.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// Password reset tokens are signed using the same HMAC key as standard access tokens but
        /// include a distinct <c>purpose</c> claim with the value <c>password_reset</c>.
        /// </para>
        /// <para>
        /// The following configuration keys are expected in <c>appsettings.json</c> under the <c>Jwt</c> section:
        /// <list type="bullet">
        /// <item><description><c>Issuer</c></description></item>
        /// <item><description><c>Audience</c></description></item>
        /// <item><description><c>SigningKey</c></description></item>
        /// <item><description><c>PasswordResetMinutes</c> (default: 15)</description></item>
        /// <item><description><c>PasswordResetAudience</c> (optional)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public (string token, DateTime expiresUtc) CreatePasswordResetToken(User user)
        {
            var jwt = config.GetSection("Jwt");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var minutes = double.TryParse(jwt["PasswordResetMinutes"], out var m) ? m : 15d;
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var audience = string.IsNullOrWhiteSpace(jwt["PasswordResetAudience"])
                ? jwt["Audience"]
                : jwt["PasswordResetAudience"];

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new("ss", user.SecurityStamp),
                new("purpose", "password_reset"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        /// <summary>
        /// Validates a password reset JWT and returns the associated <see cref="ClaimsPrincipal"/> if valid.
        /// Returns <c>null</c> if the token is invalid, expired, or not intended for password reset.
        /// </summary>
        /// <param name="token">
        /// The encoded password reset JWT to validate.
        /// </param>
        /// <returns>
        /// A <see cref="ClaimsPrincipal"/> representing the validated user identity if the token is valid;
        /// otherwise, <c>null</c> if validation fails.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Validation includes issuer, audience, signing key, and lifetime verification.
        /// A valid password reset token must contain a <c>purpose</c> claim with the value <c>password_reset</c>.
        /// </para>
        /// <para>
        /// The following configuration keys are expected in <c>appsettings.json</c> under the <c>Jwt</c> section:
        /// <list type="bullet">
        /// <item><description><c>Issuer</c></description></item>
        /// <item><description><c>Audience</c> or <c>PasswordResetAudience</c></description></item>
        /// <item><description><c>SigningKey</c></description></item>
        /// <item><description><c>ClockSkewMinutes</c> (default: 2)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public ClaimsPrincipal? ValidatePasswordResetToken(string token)
        {
            var jwt = config.GetSection("Jwt");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));

            var audience = string.IsNullOrWhiteSpace(jwt["PasswordResetAudience"])
                ? jwt["Audience"]
                : jwt["PasswordResetAudience"];

            var clockSkewMinutes = double.TryParse(jwt["ClockSkewMinutes"], out var skew) ? skew : 2d;

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt["Issuer"],

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(clockSkewMinutes)
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, parameters, out var validatedToken);

                if (principal.FindFirst("purpose")?.Value != "password_reset") return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
