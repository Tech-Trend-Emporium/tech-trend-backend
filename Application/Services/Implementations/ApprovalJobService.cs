using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.ApprovalJob;
using Application.Exceptions;
using Data.Entities;
using Domain.Constants;
using Domain.Enums;
using Domain.Validations;
using General.Dto.ApprovalJob;
using General.Dto.Category;
using General.Dto.Product;
using General.Mappers;
using System.Text.Json;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Service that coordinates creation, decision, and retrieval of approval jobs,
    /// delegating the approved operations to domain services (e.g., category/product).
    /// This class is documented by AI.
    /// </summary>
    public class ApprovalJobService : IApprovalJobService
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of <see cref="ApprovalJobService"/>.
        /// </summary>
        /// <param name="approvalJobRepository">Repository for <see cref="ApprovalJob"/> persistence.</param>
        /// <param name="unitOfWork">Unit of Work to persist and coordinate transactions.</param>
        /// <param name="categoryService">Service used to execute approved category operations.</param>
        /// <param name="productService">Service used to execute approved product operations.</param>
        public ApprovalJobService(
            IApprovalJobRepository approvalJobRepository,
            IUnitOfWork unitOfWork,
            ICategoryService categoryService,
            IProductService productService)
        {
            _approvalJobRepository = approvalJobRepository;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _productService = productService;
        }

        /// <summary>
        /// Submits a new approval job request on behalf of a requester user.
        /// </summary>
        /// <param name="requesterUserId">The ID of the user submitting the job.</param>
        /// <param name="dto">The submission payload describing type, operation, and optional data.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>The created approval job as a <see cref="ApprovalJobResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
        /// <exception cref="BadRequestException">
        /// Thrown when required fields are missing (e.g., <c>TargetId</c> for DELETE, or <c>Payload</c> for CREATE),
        /// or the payload type does not match the job type/operation.
        /// </exception>
        public async Task<ApprovalJobResponse> SubmitAsync(int requesterUserId, SubmitApprovalJobRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (dto.Operation == Operation.DELETE && !dto.TargetId.HasValue)
                throw new BadRequestException(ApprovalJobValidator.TargetIdRequiredMessage);
            if (dto.Operation == Operation.CREATE && dto.Payload is null)
                throw new BadRequestException(ApprovalJobValidator.PayloadRequiredMessage);

            string? payloadJson = null;
            if (dto.Operation == Operation.CREATE)
            {
                switch (dto.Type)
                {
                    case ApprovalJobType.CATEGORY:
                        if (dto.Payload is not CreateCategoryRequest)
                            throw new BadRequestException(ApprovalJobValidator.PayloadInvalidCategoryMessage);
                        break;
                    case ApprovalJobType.PRODUCT:
                        if (dto.Payload is not CreateProductRequest)
                            throw new BadRequestException(ApprovalJobValidator.PayloadInvalidProductMessage);
                        break;
                }
                payloadJson = JsonSerializer.Serialize(dto.Payload);
            }

            var job = new ApprovalJob
            {
                Type = dto.Type,
                Operation = dto.Operation,
                State = false,
                RequestedBy = requesterUserId,
                RequestedAt = DateTime.UtcNow,
                TargetId = dto.TargetId,
                PayloadJson = payloadJson,
                Reason = dto.Reason
            };

            _approvalJobRepository.Add(job);
            await _unitOfWork.SaveChangesAsync(ct);

            return ApprovalJobMapper.ToResponse(job);
        }

        /// <summary>
        /// Applies a decision (approve or reject) to a pending approval job.
        /// </summary>
        /// <param name="jobId">The identifier of the job to decide.</param>
        /// <param name="adminUserId">The administrator ID making the decision.</param>
        /// <param name="dto">Decision details (approve/reject and optional reason).</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>The updated job as a <see cref="ApprovalJobResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
        /// <exception cref="NotFoundException">Thrown when the job does not exist.</exception>
        /// <exception cref="ConflictException">Thrown when the job has already been decided.</exception>
        /// <exception cref="BadRequestException">
        /// Thrown when the decider is the same as the requester, or when the job type/operation is unsupported.
        /// </exception>
        public async Task<ApprovalJobResponse> DecideAsync(int jobId, int adminUserId, DecideApprovalJobRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var job = await _approvalJobRepository.GetByIdTrackedAsync(jobId, ct)
                ?? throw new NotFoundException(ApprovalJobValidator.ApprovalJobNotFoundMessage(jobId));

            if (job.DecidedAt.HasValue) throw new ConflictException(ApprovalJobValidator.ApprovalJobAlreadyDecidedMessage);
            if (job.RequestedBy == adminUserId)
                throw new BadRequestException(ApprovalJobValidator.DecidedByCannotBeSameAsRequestedByErrorMessage);

            await _unitOfWork.ExecuteInTransactionAsync(async innerCt =>
            {
                if (dto.Approve)
                {
                    await ExecuteApprovedOperation(job, innerCt);
                    job.State = true;
                }
                else
                {
                    job.State = false;
                }

                job.DecidedBy = adminUserId;
                job.DecidedAt = DateTime.UtcNow;
                job.Reason = dto.Reason;

                _approvalJobRepository.Update(job);
            }, ct);

            return ApprovalJobMapper.ToResponse(job);
        }

        /// <summary>
        /// Lists pending approval jobs, paginated.
        /// </summary>
        /// <param name="skip">Number of records to skip. Default is 0.</param>
        /// <param name="take">Number of records to take. Default is 50.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>A read-only list of pending <see cref="ApprovalJobResponse"/> objects.</returns>
        public async Task<IReadOnlyList<ApprovalJobResponse>> ListPendingAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var jobs = await _approvalJobRepository.ListPendingAsync(skip, take, ct);
            return ApprovalJobMapper.ToResponseList(jobs);
        }

        /// <summary>
        /// Retrieves a job by its unique identifier.
        /// </summary>
        /// <param name="id">The approval job identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="ApprovalJobResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ApprovalJobResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var job = await _approvalJobRepository.GetByIdAsync(ct, id);
            return job is null ? null : ApprovalJobMapper.ToResponse(job);
        }

        /// <summary>
        /// Executes the approved operation for the given job by delegating to the corresponding domain service.
        /// </summary>
        /// <param name="job">The approved job to execute.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <exception cref="BadRequestException">Thrown when the job type is unsupported.</exception>
        private async Task ExecuteApprovedOperation(ApprovalJob job, CancellationToken ct)
        {
            switch (job.Type)
            {
                case ApprovalJobType.CATEGORY:
                    await ExecuteCategory(job, ct);
                    break;

                case ApprovalJobType.PRODUCT:
                    await ExecuteProduct(job, ct);
                    break;

                default:
                    throw new BadRequestException(ApprovalJobValidator.ApprovalJobTypeNotSupportedMessage);
            }
        }

        /// <summary>
        /// Executes approved category operations (create/delete).
        /// </summary>
        /// <param name="job">The approved category job.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <exception cref="BadRequestException">Thrown for missing or invalid payload/target.</exception>
        /// <exception cref="NotFoundException">Thrown when the category to delete is not found.</exception>
        private async Task ExecuteCategory(ApprovalJob job, CancellationToken ct)
        {
            switch (job.Operation)
            {
                case Operation.CREATE:
                    if (string.IsNullOrWhiteSpace(job.PayloadJson))
                        throw new BadRequestException(ApprovalJobValidator.PayloadEmptyCategoryMessage);

                    var catDto = JsonSerializer.Deserialize<CreateCategoryRequest>(job.PayloadJson)
                        ?? throw new BadRequestException(ApprovalJobValidator.PayloadInvalidCategoryMessage);

                    await _categoryService.CreateAsync(catDto, ct);
                    break;

                case Operation.DELETE:
                    if (!job.TargetId.HasValue)
                        throw new BadRequestException(ApprovalJobValidator.TargetIdCategoryRequiredMessage);

                    var deleted = await _categoryService.DeleteAsync(job.TargetId.Value, ct);
                    if (!deleted) throw new NotFoundException(CategoryValidator.CategoryNotFound(job.TargetId.Value));
                    break;
            }
        }

        /// <summary>
        /// Executes approved product operations (create/delete).
        /// </summary>
        /// <param name="job">The approved product job.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <exception cref="BadRequestException">Thrown for missing or invalid payload/target.</exception>
        /// <exception cref="NotFoundException">Thrown when the product to delete is not found.</exception>
        private async Task ExecuteProduct(ApprovalJob job, CancellationToken ct)
        {
            switch (job.Operation)
            {
                case Operation.CREATE:
                    if (string.IsNullOrWhiteSpace(job.PayloadJson))
                        throw new BadRequestException(ApprovalJobValidator.PayloadEmptyProductMessage);

                    var prodDto = JsonSerializer.Deserialize<CreateProductRequest>(job.PayloadJson)
                        ?? throw new BadRequestException(ApprovalJobValidator.PayloadInvalidProductMessage);

                    await _productService.CreateAsync(prodDto, ct);
                    break;

                case Operation.DELETE:
                    if (!job.TargetId.HasValue)
                        throw new BadRequestException(ApprovalJobValidator.TargetIdProductRequiredMessage);

                    var ok = await _productService.DeleteAsync(job.TargetId.Value, ct);
                    if (!ok) throw new NotFoundException(ProductValidator.ProductNotFound(job.TargetId.Value));
                    break;
            }
        }
    }
}
