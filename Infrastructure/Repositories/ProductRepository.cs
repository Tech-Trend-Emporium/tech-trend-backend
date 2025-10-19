using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Product"/> entities.
    /// Provides methods for filtering, retrieving, and counting products, optionally by category.
    /// This class is documented by AI.
    /// </summary>
    public class ProductRepository : EfRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> instance used for database access.</param>
        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a single <see cref="Product"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the product.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.  
        /// Set to <c>true</c> for tracked entities, or <c>false</c> for read-only queries. Defaults to <c>false</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="Product"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<Product?> GetAsync(Expression<Func<Product, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking
                ? _db.Products.AsTracking()
                : _db.Products.AsNoTracking())
                .FirstOrDefaultAsync(predicate, ct);
        }

        /// <summary>
        /// Retrieves a list of products whose identifiers match the provided list of IDs.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of product IDs to filter by. If <c>null</c> or empty, an empty list is returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of matching <see cref="Product"/> entities.
        /// </returns>
        public Task<IReadOnlyList<Product>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            if (ids == null || !ids.Any())
                return Task.FromResult((IReadOnlyList<Product>)new List<Product>());

            var products = _db.Products.Where(c => ids.Contains(c.Id)).ToList();
            return Task.FromResult((IReadOnlyList<Product>)products);
        }

        /// <summary>
        /// Retrieves a paginated list of products, optionally filtered by category ID.
        /// </summary>
        /// <param name="skip">The number of records to skip before collecting results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="categoryId">
        /// An optional category ID used to filter products.  
        /// If <c>null</c>, all products are retrieved.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="Product"/> entities.
        /// </returns>
        public Task<IReadOnlyList<Product>> ListAsync(int skip = 0, int take = 50, int? categoryId = null, CancellationToken ct = default)
        {
            if (categoryId.HasValue)
                return base.ListAsync(p => p.CategoryId == categoryId.Value, skip, take, ct);

            return base.ListAsync(skip, take, ct);
        }

        /// <summary>
        /// Counts the number of products, optionally filtered by category ID.
        /// </summary>
        /// <param name="categoryId">
        /// An optional category ID used to filter products.  
        /// If <c>null</c>, counts all products in the database.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the count of matching <see cref="Product"/> entities.
        /// </returns>
        public Task<int> CountAsync(int? categoryId = null, CancellationToken ct = default)
        {
            if (categoryId.HasValue)
                return base.CountAsync(p => p.CategoryId == categoryId.Value, ct);

            return base.CountAsync(null, ct);
        }
    }
}
