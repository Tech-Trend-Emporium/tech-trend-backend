using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Coupon"/> entities.
    /// Provides data access methods for retrieving and querying coupon data.
    /// This class is documented by AI.
    /// </summary>
    public class CouponRepository : EfRepository<Coupon>, ICouponRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public CouponRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a single <see cref="Coupon"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the coupon.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.  
        /// Set to <c>true</c> for tracked entities, or <c>false</c> for read-only queries.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="Coupon"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<Coupon?> GetAsync(Expression<Func<Coupon, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking
                ? _db.Coupons.AsTracking()
                : _db.Coupons.AsNoTracking())
                .FirstOrDefaultAsync(predicate, ct);
        }
    }
}
