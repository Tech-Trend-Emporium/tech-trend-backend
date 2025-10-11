using Data.Entities;
using System.Linq.Expressions;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Product"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IProductRepository : IEfRepository<Product>
    {
        /// <summary>
        /// Retrieves a single <see cref="Product"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the <see cref="Product"/> entity.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.
        /// Set to <c>true</c> for tracked entities, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="Product"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Product?> GetAsync(Expression<Func<Product, bool>> predicate, bool asTracking = false, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of <see cref="Product"/> entities whose identifiers match the provided list of IDs.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of product IDs to filter by. If <c>null</c>, an empty list will be returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of matching <see cref="Product"/> entities.
        /// </returns>
        Task<IReadOnlyList<Product>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);

        /// <summary>
        /// Retrieves a paginated list of <see cref="Product"/> entities, optionally filtered by category.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="categoryId">
        /// The optional category ID used to filter products. 
        /// If <c>null</c>, all products are included.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of <see cref="Product"/> entities.
        /// </returns>
        Task<IReadOnlyList<Product>> ListAsync(int skip = 0, int take = 50, int? categoryId = null, CancellationToken ct = default);

        /// <summary>
        /// Counts the number of <see cref="Product"/> entities, optionally filtered by category.
        /// </summary>
        /// <param name="categoryId">
        /// The optional category ID used to filter products.
        /// If <c>null</c>, counts all products.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the number of matching <see cref="Product"/> entities.
        /// </returns>
        Task<int> CountAsync(int? categoryId = null, CancellationToken ct = default);
    }
}
