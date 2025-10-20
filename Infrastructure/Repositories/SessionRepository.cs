using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Session"/> entities.
    /// Provides methods for retrieving user sessions, including active session filtering.
    /// This class is documented by AI.
    /// </summary>
    public class SessionRepository : EfRepository<Session>, ISessionRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public SessionRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves all active sessions associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active sessions are being retrieved.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a list of active <see cref="Session"/> entities for the specified user.
        /// </returns>
        public async Task<List<Session>> GetActiveByUserAsync(int userId, CancellationToken ct = default)
        {
            return await _db.Sessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync(ct);
        }
    }
}
