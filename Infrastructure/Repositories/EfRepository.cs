using Application.Abstraction;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Provides a generic implementation of the <see cref="IEfRepository{T}"/> interface
    /// for performing common CRUD operations using Entity Framework Core.
    /// This class is documented by AI.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepository{T}"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public EfRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        /// <summary>
        /// Adds a new entity to the context.
        /// </summary>
        /// <param name="entity">The entity instance to add.</param>
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Counts the number of entities that match an optional filter expression.
        /// </summary>
        /// <param name="predicate">An optional expression used to filter entities.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the number of matching entities.
        /// </returns>
        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate != null)
                return _dbSet.CountAsync(predicate, ct);

            return _dbSet.CountAsync(ct);
        }

        /// <summary>
        /// Deletes an entity by its primary key values.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The primary key values identifying the entity.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if the entity was found and deleted; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            var entity = await _dbSet.FindAsync(keyValues, ct).AsTask();
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        /// <summary>
        /// Determines whether any entity exists that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression to test entities against.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if any entity matches; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().AnyAsync(predicate, ct);
        }

        /// <summary>
        /// Retrieves an entity by its primary key values.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="keyValues">The primary key values identifying the entity.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the entity if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            return _dbSet.FindAsync(keyValues, ct).AsTask();
        }

        /// <summary>
        /// Retrieves a paginated list of entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of entities.
        /// </returns>
        public Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct)
                .ContinueWith<IReadOnlyList<T>>(t => (IReadOnlyList<T>)t.Result, ct);
        }

        /// <summary>
        /// Retrieves a paginated list of entities that match a specific predicate.
        /// </summary>
        /// <param name="predicate">The filter expression used to select entities.</param>
        /// <param name="skip">The number of records to skip before starting to collect results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of entities matching the predicate.
        /// </returns>
        public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking()
                .Where(predicate)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct)
                .ContinueWith<IReadOnlyList<T>>(t => (IReadOnlyList<T>)t.Result, ct);
        }

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">The entity instance to remove.</param>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity instance to update.</param>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
