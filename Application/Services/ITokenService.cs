using Data.Entities;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for creating and managing authentication tokens, 
    /// including access and refresh tokens.
    /// This interface is documented by AI.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new JSON Web Token (JWT) access token for the specified user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> entity for whom the access token will be created.</param>
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
        /// <param name="userId">The unique identifier of the user for whom the refresh token is being generated.</param>
        /// <param name="sessionId">The unique identifier of the user session associated with the refresh token.</param>
        /// <returns>
        /// A new instance of the <see cref="RefreshToken"/> entity representing the generated token.
        /// </returns>
        RefreshToken CreateRefreshToken(int userId, int sessionId);
    }
}
