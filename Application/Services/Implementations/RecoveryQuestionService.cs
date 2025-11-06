using Application.Abstractions;
using Application.Dtos.RecoveryQuestion;
using Application.Exceptions;
using Application.Mappers;
using Domain.Validations;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing recovery questions (catalog),
    /// including creation, retrieval, listing, updates, existence checks, and deletion.
    /// This class is documented by AI.
    /// </summary>
    public class RecoveryQuestionService : IRecoveryQuestionService
    {
        private readonly IRecoveryQuestionRepository _recoveryQuestionRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryQuestionService"/> class.
        /// </summary>
        /// <param name="repository">Repository for recovery question persistence and retrieval.</param>
        /// <param name="unitOfWork">Unit of Work to manage transactions and save changes.</param>
        public RecoveryQuestionService(IRecoveryQuestionRepository recoveryQuestionRepository, IUnitOfWork unitOfWork)
        {
            _recoveryQuestionRepository = recoveryQuestionRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns the total number of recovery questions stored in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The total count of recovery questions.</returns>
        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _recoveryQuestionRepository.CountAsync(null, ct);
        }

        /// <summary>
        /// Creates a new recovery question.
        /// </summary>
        /// <param name="dto">The data transfer object containing recovery question details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The created <see cref="RecoveryQuestionResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="ConflictException">Thrown when a recovery question with the same text already exists.</exception>
        public async Task<RecoveryQuestionResponse> CreateAsync(CreateRecoveryQuestionRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var normalized = (dto.Question ?? string.Empty).Trim();
            var exists = await _recoveryQuestionRepository.ExistsAsync(q => q.Question == normalized, ct);
            if (exists) throw new ConflictException(RecoveryQuestionValidator.QuestionAlreadyExistsMessage);

            var entity = RecoveryQuestionMapper.ToEntity(dto);
            _recoveryQuestionRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(ct);
            return RecoveryQuestionMapper.ToResponse(entity);
        }

        /// <summary>
        /// Deletes a recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The recovery question ID.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns><c>true</c> if deleted successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _recoveryQuestionRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Retrieves a recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The recovery question ID.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A <see cref="RecoveryQuestionResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<RecoveryQuestionResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _recoveryQuestionRepository.GetByIdAsync(ct, id);
            return entity is null ? null : RecoveryQuestionMapper.ToResponse(entity);
        }

        /// <summary>
        /// Retrieves a paginated list of recovery questions.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>A read-only list of <see cref="RecoveryQuestionResponse"/> objects.</returns>
        public async Task<IReadOnlyList<RecoveryQuestionResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var list = await _recoveryQuestionRepository.ListAsync(skip, take, ct);
            return RecoveryQuestionMapper.ToResponseList(list);
        }

        /// <summary>
        /// Retrieves a paginated list of recovery questions along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>
        /// A tuple containing the list of <see cref="RecoveryQuestionResponse"/> objects and the total count of recovery questions.
        /// </returns>
        public async Task<(IReadOnlyList<RecoveryQuestionResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _recoveryQuestionRepository.ListAsync(skip, take, ct);
            var total = await _recoveryQuestionRepository.CountAsync(null, ct);

            var items = RecoveryQuestionMapper.ToResponseList(listTask);

            return (items, total);
        }

        /// <summary>
        /// Determines whether a recovery question with the specified text already exists.
        /// </summary>
        /// <param name="question">The question text to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns><c>true</c> if a question with the specified text exists; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsByTextAsync(string question, CancellationToken ct = default)
        {
            var normalized = (question ?? string.Empty).Trim();
            return _recoveryQuestionRepository.ExistsAsync(q => q.Question == normalized, ct);
        }

        /// <summary>
        /// Updates an existing recovery question with new data.
        /// </summary>
        /// <param name="id">The recovery question ID to update.</param>
        /// <param name="dto">The data transfer object containing new question information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> for task cancellation.</param>
        /// <returns>The updated <see cref="RecoveryQuestionResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the recovery question does not exist.</exception>
        /// <exception cref="ConflictException">Thrown when the updated question text already exists on a different record.</exception>
        public async Task<RecoveryQuestionResponse> UpdateAsync(int id, UpdateRecoveryQuestionRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _recoveryQuestionRepository.GetByIdAsync(ct, id)
                ?? throw new NotFoundException(RecoveryQuestionValidator.RecoveryQuestionNotFound(id));

            var normalized = dto.Question.Trim();
            var duplicate = await _recoveryQuestionRepository.ExistsAsync(q => q.Id != id && q.Question == normalized, ct);
            if (duplicate) throw new ConflictException(RecoveryQuestionValidator.QuestionAlreadyExistsMessage);

            RecoveryQuestionMapper.ApplyUpdate(entity, dto);
            _recoveryQuestionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return RecoveryQuestionMapper.ToResponse(entity);
        }
    }
}
