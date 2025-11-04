using Data.Entities;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for creating and managing authentication tokens, 
    /// including access, refresh, and password-reset tokens.
    /// This interface is documented by AI.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new JSON Web Token (JWT) access token for the specified user.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> entity for whom the access token will be created.
        /// </param>
        /// <param name="extraClaims">
        /// An optional collection of additional <see cref="Claim"/> objects to include in the token payload.
        /// This can be used to embed custom information such as roles or permissions.
        /// </param>
        /// <returns>
        /// A tuple containing the generated access token string and its expiration date in UTC.
        /// </returns>
        (string accessToken, DateTime expiresUtc) CreateAccessToken(User user, IEnumerable<Claim>? extraClaims = null);

        /// <summary>
        /// Creates a new refresh token associated with the specified user and session.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user for whom the refresh token is being generated.
        /// </param>
        /// <param name="sessionId">
        /// The unique identifier of the user session associated with the refresh token.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="RefreshToken"/> entity representing the generated token.
        /// </returns>
        RefreshToken CreateRefreshToken(int userId, int sessionId);

        /// <summary>
        /// Generates a short-lived JWT used specifically for password reset operations.
        /// The token embeds the user's identifier, email, and a purpose claim for validation.
        /// </summary>
        /// <param name="user">
        /// The <see cref="User"/> entity for whom the password reset token will be created.
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <c>token</c> — the generated password reset token string.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <c>expiresUtc</c> — the UTC expiration date for the token.
        /// </description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Password reset tokens are typically valid for a short duration (e.g., 10–15 minutes)
        /// and can be invalidated when the user's <see cref="User.SecurityStamp"/> changes.
        /// </remarks>
        (string token, DateTime expiresUtc) CreatePasswordResetToken(User user);

        /// <summary>
        /// Validates a password reset token and returns the associated <see cref="ClaimsPrincipal"/> if valid.
        /// </summary>
        /// <param name="token">
        /// The password reset token string to validate.
        /// </param>
        /// <returns>
        /// A <see cref="ClaimsPrincipal"/> representing the validated user identity if the token is valid;
        /// otherwise, <c>null</c> if the token is invalid, expired, or not intended for password reset.
        /// </returns>
        /// <remarks>
        /// Implementations should verify the token signature, issuer, audience, expiration, 
        /// and the presence of the <c>purpose=password_reset</c> claim.
        /// </remarks>
        ClaimsPrincipal? ValidatePasswordResetToken(string token);
    }
}
