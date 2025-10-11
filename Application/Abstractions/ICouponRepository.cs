using Data.Entities;
using System.Linq.Expressions;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Coupon"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICouponRepository : IEfRepository<Coupon>
    {
        /// <summary>
        /// Retrieves a single <see cref="Coupon"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the <see cref="Coupon"/> entity.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.
        /// Set to <c>true</c> to enable change tracking, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="Coupon"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Coupon?> GetAsync(Expression<Func<Coupon, bool>> predicate, bool asTracking = false, CancellationToken ct = default);
    }
}
