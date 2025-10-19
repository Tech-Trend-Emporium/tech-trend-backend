using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Category"/> entities.
    /// Provides data access operations specific to categories.
    /// This class is documented by AI.
    /// </summary>
    public class CategoryRepository : EfRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> instance for database access.</param>
        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves a single <see cref="Category"/> entity that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to locate the category.</param>
        /// <param name="asTracking">
        /// Indicates whether the entity should be tracked by the Entity Framework context.  
        /// Set to <c>true</c> to enable change tracking, or <c>false</c> for read-only queries.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="Category"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<Category?> GetAsync(Expression<Func<Category, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking ? _db.Categories.AsTracking() : _db.Categories.AsNoTracking())
                .FirstOrDefaultAsync(predicate, ct);
        }

        /// <summary>
        /// Retrieves a list of categories whose IDs match the provided list of identifiers.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="ids">The list of category IDs to retrieve. If <c>null</c>, an empty list is returned.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="Category"/> entities.
        /// </returns>
        public Task<IReadOnlyList<Category>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            if (ids == null || !ids.Any())
                return Task.FromResult((IReadOnlyList<Category>)new List<Category>());

            var categories = _db.Categories.Where(c => ids.Contains(c.Id)).ToList();
            return Task.FromResult((IReadOnlyList<Category>)categories);
        }
    }
}
