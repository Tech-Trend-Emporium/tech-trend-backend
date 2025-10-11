using Application.Dtos.ApprovalJob;
using General.Dto.ApprovalJob;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing approval job operations within the application layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IApprovalJobService
    {
        /// <summary>
        /// Submits a new approval job request on behalf of a specific user.
        /// </summary>
        /// <param name="requesterUserId">The unique identifier of the user submitting the approval job.</param>
        /// <param name="dto">The data transfer object containing details of the approval job request.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains an <see cref="ApprovalJobResponse"/> representing the created approval job.
        /// </returns>
        Task<ApprovalJobResponse> SubmitAsync(int requesterUserId, SubmitApprovalJobRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Processes a decision (approval or rejection) for an existing approval job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the approval job to decide on.</param>
        /// <param name="adminUserId">The unique identifier of the administrator making the decision.</param>
        /// <param name="dto">The data transfer object containing the decision details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains an updated <see cref="ApprovalJobResponse"/> reflecting the decision outcome.
        /// </returns>
        Task<ApprovalJobResponse> DecideAsync(int jobId, int adminUserId, DecideApprovalJobRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of pending approval jobs awaiting a decision.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of <see cref="ApprovalJobResponse"/> objects for pending approval jobs.
        /// </returns>
        Task<IReadOnlyList<ApprovalJobResponse>> ListPendingAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific approval job by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the approval job.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains an <see cref="ApprovalJobResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ApprovalJobResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
