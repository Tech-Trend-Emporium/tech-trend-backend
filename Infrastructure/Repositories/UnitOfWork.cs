using Application.Abstractions;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implements the Unit of Work pattern for coordinating database operations
    /// and ensuring transactional integrity across multiple repository actions.
    /// This class is documented by AI.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for data persistence and transaction management.</param>
        public UnitOfWork(AppDbContext db) => _db = db;

        /// <summary>
        /// Persists all tracked changes to the database.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the number of state entries written to the database.
        /// </returns>
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Executes a specified asynchronous action within a database transaction.
        /// Commits the transaction if the operation succeeds; rolls it back on failure.
        /// </summary>
        /// <param name="action">A delegate representing the asynchronous operation to execute within the transaction.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">
        /// Propagates any exception thrown during the transaction after rolling back the transaction.
        /// </exception>
        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                await action(ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
