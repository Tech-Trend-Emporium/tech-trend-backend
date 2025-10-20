using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Cart"/> entities.
    /// Provides methods for retrieving and creating user-specific shopping carts.
    /// This class is documented by AI.
    /// </summary>
    public class CartRepository : EfRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public CartRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new shopping cart for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the cart is being created.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the newly created <see cref="Cart"/> entity.
        /// </returns>
        public async Task<Cart> CreateForUserAsync(int userId, CancellationToken ct = default)
        {
            var cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };

            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);

            return cart;
        }

        /// <summary>
        /// Retrieves the cart associated with the specified user, optionally including related entities.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is being retrieved.</param>
        /// <param name="includeGraph">
        /// A flag indicating whether to include related entities such as items, products, and applied coupons.
        /// Defaults to <c>true</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the user's <see cref="Cart"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<Cart?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default)
        {
            IQueryable<Cart> q = _db.Carts.Where(c => c.UserId == userId);

            if (includeGraph)
                q = q.Include(c => c.Items)
                     .ThenInclude(i => i.Product)
                     .Include(c => c.Coupon);

            return await q.FirstOrDefaultAsync(ct);
        }
    }
}
