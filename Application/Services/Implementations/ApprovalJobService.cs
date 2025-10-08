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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ApprovalJobService : IApprovalJobService
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public ApprovalJobService(IApprovalJobRepository approvalJobRepository, IUnitOfWork unitOfWork, ICategoryService categoryService, IProductService productService)
        {
            _approvalJobRepository = approvalJobRepository;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<ApprovalJobResponse> SubmitAsync(int requesterUserId, SubmitApprovalJobRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (dto.Operation == Operation.DELETE && !dto.TargetId.HasValue) throw new BadRequestException("TargetId es obligatorio para DELETE.");
            if (dto.Operation == Operation.CREATE && dto.Payload is null) throw new BadRequestException("Payload es obligatorio para CREATE.");

            string? payloadJson = null;
            if (dto.Operation == Operation.CREATE)
            {
                switch (dto.Type)
                {
                    case ApprovalJobType.CATEGORY:
                        if (dto.Payload is not CreateCategoryRequest) throw new BadRequestException("Payload inválido para CATEGORY/CREATE.");
                        break;
                    case ApprovalJobType.PRODUCT:
                        if (dto.Payload is not CreateProductRequest) throw new BadRequestException("Payload inválido para PRODUCT/CREATE.");
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

        public async Task<ApprovalJobResponse> DecideAsync(int jobId, int adminUserId, DecideApprovalJobRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var job = await _approvalJobRepository.GetByIdTrackedAsync(jobId, ct) ?? throw new NotFoundException($"ApprovalJob {jobId} no encontrado.");

            if (job.DecidedAt.HasValue) throw new ConflictException("Este ApprovalJob ya fue decidido.");
            if (job.RequestedBy == adminUserId) throw new BadRequestException(ApprovalJobValidator.DecidedByCannotBeSameAsRequestedByErrorMessage);

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

        public async Task<IReadOnlyList<ApprovalJobResponse>> ListPendingAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var jobs = await _approvalJobRepository.ListPendingAsync(skip, take, ct);

            return ApprovalJobMapper.ToResponseList(jobs);
        }

        public async Task<ApprovalJobResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var job = await _approvalJobRepository.GetByIdAsync(ct, id);

            return job is null ? null : ApprovalJobMapper.ToResponse(job);
        }

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
                    throw new BadRequestException("Tipo de ApprovalJob no soportado.");
            }
        }

        private async Task ExecuteCategory(ApprovalJob job, CancellationToken ct)
        {
            switch (job.Operation)
            {
                case Operation.CREATE:
                    if (string.IsNullOrWhiteSpace(job.PayloadJson)) throw new BadRequestException("Payload vacío para CATEGORY/CREATE.");

                    var catDto = JsonSerializer.Deserialize<CreateCategoryRequest>(job.PayloadJson) ?? throw new BadRequestException("Payload inválido para CATEGORY/CREATE.");

                    await _categoryService.CreateAsync(catDto, ct);
                    break;

                case Operation.DELETE:
                    if (!job.TargetId.HasValue) throw new BadRequestException("TargetId requerido para CATEGORY/DELETE.");

                    var deleted = await _categoryService.DeleteAsync(job.TargetId.Value, ct);
                    if (!deleted) throw new NotFoundException(CategoryValidator.CategoryNotFound(job.TargetId.Value));
                    break;
            }
        }

        private async Task ExecuteProduct(ApprovalJob job, CancellationToken ct)
        {
            switch (job.Operation)
            {
                case Operation.CREATE:
                    if (string.IsNullOrWhiteSpace(job.PayloadJson)) throw new BadRequestException("Payload vacío para PRODUCT/CREATE.");

                    var prodDto = JsonSerializer.Deserialize<CreateProductRequest>(job.PayloadJson) ?? throw new BadRequestException("Payload inválido para PRODUCT/CREATE.");

                    await _productService.CreateAsync(prodDto, ct);
                    break;

                case Operation.DELETE:
                    if (!job.TargetId.HasValue) throw new BadRequestException("TargetId requerido para PRODUCT/DELETE.");

                    var ok = await _productService.DeleteAsync(job.TargetId.Value, ct);
                    if (!ok) throw new NotFoundException(ProductValidator.ProductNotFound(job.TargetId.Value));
                    break;
            }
        }
    }
}
