using System.Linq.Expressions;

namespace Application.Abstraction
{
    /// <summary>
    /// Defines a generic contract for Entity Framework-based repository operations.
    /// This interface is documented by AI.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public interface IEfRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its primary key values.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The primary key values identifying the entity.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues);

        /// <summary>
        /// Retrieves a paginated list of all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of entities.
        /// </returns>
        Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate entities.</param>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of matching entities.
        /// </returns>
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Determines whether any entity exists that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression to test entities against.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if any matching entity exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        /// <summary>
        /// Counts the number of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">
        /// An optional filter expression used to count entities.
        /// If <c>null</c>, counts all entities.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the number of matching entities.
        /// </returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        /// <summary>
        /// Deletes an entity by its primary key values.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The primary key values identifying the entity to delete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteByIdAsync(CancellationToken ct = default, params object[] keyValues);

        /// <summary>
        /// Adds a new entity to the context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);
    }
}
