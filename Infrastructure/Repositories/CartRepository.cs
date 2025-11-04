using Application.Abstraction;
using Data.Entities;
using Domain.Enums;
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

        /// <summary>
        /// Lists placed orders (carts with <c>Status = PLACED</c>) for a user, including items, products, and coupon.
        /// Results are ordered by <see cref="Cart.PlacedAtUtc"/> descending.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="skip">Number of records to skip. Defaults to 0.</param>
        /// <param name="take">Number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/>.</param>
        /// <returns>A read-only list of placed <see cref="Cart"/> entities.</returns>
        public async Task<IReadOnlyList<Cart>> ListPlacedByUserAsync(
            int userId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return await _db.Carts
                .Where(c => c.UserId == userId && c.Status == CartStatus.PLACED)
                .Include(c => c.Items).ThenInclude(i => i.Product)
                .Include(c => c.Coupon)
                .OrderByDescending(c => c.PlacedAtUtc) 
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        /// <summary>
        /// Counts placed orders (carts with <c>Status = PLACED</c>) for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/>.</param>
        /// <returns>Total number of placed carts for the user.</returns>
        public Task<int> CountPlacedByUserAsync(int userId, CancellationToken ct = default)
        {
            return _db.Carts.CountAsync(c => c.UserId == userId && c.Status == CartStatus.PLACED, ct);
        }
    }
}
