using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="WishList"/> entities.
    /// Provides data access methods for retrieving and creating user-specific wish lists.
    /// This class is documented by AI.
    /// </summary>
    public class WishListRepository : EfRepository<WishList>, IWishListRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database operations.</param>
        public WishListRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves the wish list associated with a specific user.
        /// Optionally includes related entities such as items and products.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list is being retrieved.</param>
        /// <param name="includeGraph">
        /// Indicates whether to include related navigation properties (e.g., items and products) 
        /// in the query result. Defaults to <c>true</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="WishList"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<WishList?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default)
        {
            IQueryable<WishList> q = _db.WishLists.Where(w => w.UserId == userId);

            if (includeGraph)
                q = q.Include(w => w.Items)
                     .ThenInclude(i => i.Product);

            return await q.FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Creates a new wish list for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the wish list will be created.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the newly created <see cref="WishList"/> entity.
        /// </returns>
        public async Task<WishList> CreateForUserAsync(int userId, CancellationToken ct = default)
        {
            var wl = new WishList
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.WishLists.Add(wl);
            await _db.SaveChangesAsync(ct);

            return wl;
        }
    }
}
