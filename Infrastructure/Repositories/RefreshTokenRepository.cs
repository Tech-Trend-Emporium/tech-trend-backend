using Application.Abstractions;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="RefreshToken"/> entities.
    /// Provides methods for token lookup, revocation, and mass deactivation of active tokens.
    /// This class is documented by AI.
    /// </summary>
    public class RefreshTokenRepository : EfRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> instance used for database access.</param>
        public RefreshTokenRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a <see cref="RefreshToken"/> entity by its token string.
        /// </summary>
        /// <param name="token">The refresh token value to look up.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="RefreshToken"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, ct);
        }

        /// <summary>
        /// Revokes all currently active refresh tokens for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active tokens will be revoked.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <remarks>
        /// This method uses a bulk update operation (<see cref="ExecuteUpdateAsync"/>) for efficiency.
        /// It sets the <c>RevokedAtUtc</c> timestamp for all valid (non-expired, non-revoked) tokens.
        /// </remarks>
        public async Task RevokeAllActiveByUserAsync(int userId, CancellationToken ct = default)
        {
            var q = _db.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAtUtc == null && r.ExpiresAtUtc > DateTime.UtcNow);

            await q.ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAtUtc, DateTime.UtcNow), ct);
        }

        /// <summary>
        /// Revokes a specific refresh token identified by its token string.
        /// </summary>
        /// <param name="token">The refresh token value to revoke.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <remarks>
        /// Only active and non-expired tokens are affected.  
        /// This method performs a direct database update without loading the entity into memory.
        /// </remarks>
        public async Task RevokeByTokenAsync(string token, CancellationToken ct = default)
        {
            var q = _db.RefreshTokens
                .Where(r => r.Token == token && r.RevokedAtUtc == null && r.ExpiresAtUtc > DateTime.UtcNow);

            await q.ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAtUtc, DateTime.UtcNow), ct);
        }
    }
}
