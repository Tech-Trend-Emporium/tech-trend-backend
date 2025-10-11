namespace Application.Abstractions
{
    /// <summary>
    /// Defines the contract for a Unit of Work, which coordinates the writing of changes 
    /// across multiple repositories in a single transaction.
    /// This interface is documented by AI.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists all tracked changes to the database.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the number of state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Executes a given asynchronous action within a database transaction.
        /// If the action completes successfully, the transaction is committed; 
        /// otherwise, it is rolled back.
        /// </summary>
        /// <param name="action">
        /// A function representing the asynchronous operation to execute within the transaction.
        /// The function receives a <see cref="CancellationToken"/> parameter.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
    }
}
