using Application.Dtos.RecoveryQuestion;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing recovery question catalog entries and related business logic.
    /// This interface is documented by AI.
    /// </summary>
    public interface IRecoveryQuestionService
    {
        /// <summary>
        /// Retrieves a specific recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the recovery question.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the <see cref="RecoveryQuestionResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<RecoveryQuestionResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of recovery questions.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a read-only list of <see cref="RecoveryQuestionResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<RecoveryQuestionResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of recovery questions along with the total number of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a tuple with the list of <see cref="RecoveryQuestionResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<RecoveryQuestionResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of recovery questions available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the total number of recovery questions.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);

        /// <summary>
        /// Creates a new recovery question based on the provided request data.
        /// </summary>
        /// <param name="dto">The data transfer object containing recovery question creation details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the newly created <see cref="RecoveryQuestionResponse"/>.
        /// </returns>
        Task<RecoveryQuestionResponse> CreateAsync(CreateRecoveryQuestionRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing recovery question with the specified identifier using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the recovery question to update.</param>
        /// <param name="dto">The data transfer object containing updated recovery question information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="RecoveryQuestionResponse"/>.
        /// </returns>
        Task<RecoveryQuestionResponse> UpdateAsync(int id, UpdateRecoveryQuestionRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the recovery question to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if the recovery question was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a recovery question with the specified text already exists.
        /// </summary>
        /// <param name="question">The question text to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains <c>true</c> if a recovery question with the specified text exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByTextAsync(string question, CancellationToken ct = default);
    }
}
