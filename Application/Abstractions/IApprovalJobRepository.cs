using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="ApprovalJob"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IApprovalJobRepository : IEfRepository<ApprovalJob>
    {
        /// <summary>
        /// Retrieves an <see cref="ApprovalJob"/> entity by its unique identifier
        /// with tracking enabled in the Entity Framework context.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="ApprovalJob"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task result contains the <see cref="ApprovalJob"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ApprovalJob?> GetByIdTrackedAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of <see cref="ApprovalJob"/> entities that are pending approval.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set.</param>
        /// <param name="take">The number of records to take for the result set.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of pending <see cref="ApprovalJob"/> entities.
        /// </returns>
        Task<IReadOnlyList<ApprovalJob>> ListPendingAsync(int skip, int take, CancellationToken ct = default);
    }
}
