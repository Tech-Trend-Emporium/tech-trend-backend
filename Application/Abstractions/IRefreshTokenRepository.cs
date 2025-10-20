using Application.Abstraction;
using Domain.Entities;

namespace Application.Abstractions
{
    /// <summary>
    /// Interface for managing <see cref="RefreshToken"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IRefreshTokenRepository : IEfRepository<RefreshToken>
    {
        /// <summary>
        /// Retrieves a <see cref="RefreshToken"/> entity that matches the specified token string.
        /// </summary>
        /// <param name="token">The token value used to locate the <see cref="RefreshToken"/> entity.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="RefreshToken"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);

        /// <summary>
        /// Revokes all active refresh tokens associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active tokens will be revoked.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RevokeAllActiveByUserAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Revokes a specific refresh token identified by its token string.
        /// </summary>
        /// <param name="token">The token value identifying the refresh token to revoke.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RevokeByTokenAsync(string token, CancellationToken ct = default);
    }
}
