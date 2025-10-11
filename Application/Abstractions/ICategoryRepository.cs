using Data.Entities;
using System.Linq.Expressions;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Category"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICategoryRepository : IEfRepository<Category>
    {
        /// <summary>
        /// Retrieves a single <see cref="Category"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the <see cref="Category"/> entity.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.
        /// Set to <c>true</c> for tracked entities, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="Category"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Category?> GetAsync(Expression<Func<Category, bool>> predicate, bool asTracking = false, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of <see cref="Category"/> entities whose identifiers match the provided list of IDs.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of category IDs to filter by. If <c>null</c>, an empty list will be returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of matching <see cref="Category"/> entities.
        /// </returns>
        Task<IReadOnlyList<Category>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
    }
}
