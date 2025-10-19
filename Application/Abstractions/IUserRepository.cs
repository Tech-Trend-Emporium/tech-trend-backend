using Data.Entities;
using System.Linq.Expressions;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="User"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IUserRepository : IEfRepository<User>
    {
        /// <summary>
        /// Retrieves a single <see cref="User"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the <see cref="User"/> entity.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.
        /// Set to <c>true</c> for tracked entities, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="User"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<User?> GetAsync(Expression<Func<User, bool>> predicate, bool asTracking = false, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of <see cref="User"/> entities whose identifiers match the provided list of IDs.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of user IDs to filter by. If <c>null</c>, an empty list will be returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of matching <see cref="User"/> entities.
        /// </returns>
        Task<IReadOnlyList<User>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
    }
}
