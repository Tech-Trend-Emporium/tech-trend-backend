using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="User"/> entities.
    /// Provides methods for retrieving users by predicate or by ID list.
    /// This class is documented by AI.
    /// </summary>
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public UserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a single <see cref="User"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the user.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.  
        /// Set to <c>true</c> to enable change tracking, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="User"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<User?> GetAsync(Expression<Func<User, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking
                ? _db.Users.AsTracking()
                : _db.Users.AsNoTracking())
                .FirstOrDefaultAsync(predicate, ct);
        }

        /// <summary>
        /// Retrieves a list of <see cref="User"/> entities whose IDs match the provided list of identifiers.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of user IDs to filter by. If <c>null</c> or empty, an empty list is returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of matching <see cref="User"/> entities.
        /// </returns>
        public Task<IReadOnlyList<User>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            if (ids == null || !ids.Any())
                return Task.FromResult((IReadOnlyList<User>)new List<User>());

            var users = _db.Users.Where(u => ids.Contains(u.Id)).ToList();
            return Task.FromResult((IReadOnlyList<User>)users);
        }
    }
}
