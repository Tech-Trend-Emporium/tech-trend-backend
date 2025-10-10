using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.ApprovalJob;
using Application.Exceptions;
using Application.Services;
using Data.Entities;
using Domain.Enums;
using General.Dto.Category;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnitTests.ApprovalJobService
{
    public class ApprovalServiceDecideTests
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly Application.Services.Implementations.ApprovalJobService _sut;

        public ApprovalServiceDecideTests()
        {
            _approvalJobRepository = Substitute.For<IApprovalJobRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = Substitute.For<ICategoryService>();
            _productService = Substitute.For<IProductService>();

            _sut = new Application.Services.Implementations.ApprovalJobService(_approvalJobRepository, _unitOfWork, _categoryService, _productService);
        }

        [Fact]
        public async Task DecideAsync_ShouldThrowArgumentNull_WhenDtoIsNull()
        {
            int jobId = 1;
            int adminUserId = 10;
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DecideAsync(jobId, adminUserId, null!, CancellationToken.None));
        }

        [Fact]
        public async Task DecideAsync_ShouldThrowNotFound_WhenJobDoesNotExist()
        {
            int jobId = 1;
            int adminUserId = 10;
            var ct = CancellationToken.None;
            _approvalJobRepository.GetByIdTrackedAsync(jobId, ct).Returns((ApprovalJob?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.DecideAsync(jobId, adminUserId, new DecideApprovalJobRequest { Approve = true }, ct));

        }

        [Fact]
        public async Task DecideAsync_ShouldThrowConflict_WhenAlreadyDecided()
        {
            int jobId = 1;
            int adminUserId = 20;   
            var ct = CancellationToken.None;
            var job = new ApprovalJob { Id = 1, DecidedAt = DateTime.UtcNow };
            _approvalJobRepository.GetByIdTrackedAsync(jobId, ct).Returns(job);

            await Assert.ThrowsAsync<ConflictException>(
                () => _sut.DecideAsync(1, 20, new DecideApprovalJobRequest { Approve = true }, CancellationToken.None));
        }

        [Fact]
        public async Task DecideAsync_ShouldThrowBadRequest_WhenRequestedBySameUser()
        {
            int jobId = 1;
            int adminUserId = 10;
            var ct = CancellationToken.None;
            var job = new ApprovalJob { Id = 1, RequestedBy = 10, State = true };
            _approvalJobRepository.GetByIdTrackedAsync(jobId, ct).Returns(job);

            await Assert.ThrowsAsync<BadRequestException>(
                () => _sut.DecideAsync(jobId, adminUserId, new DecideApprovalJobRequest { Approve = true }, ct));
        }

        [Fact]
        public async Task DecideAsync_ShouldApproveAndUpdateJob_WhenApproveIsTrue()
        {
            // Arrange
            int jobId = 1;
            int adminUserId = 20;
            var ct = CancellationToken.None;
            var job = new ApprovalJob { Id = jobId, RequestedBy = 5, Type = ApprovalJobType.CATEGORY, PayloadJson = System.Text.Json.JsonSerializer.Serialize("Electronics") };
            var dto = new DecideApprovalJobRequest { Approve = true, Reason = "Approved by admin" };

            var baseRequest = new SubmitApprovalJobRequest
            {
                Operation = Operation.CREATE,
                Type = ApprovalJobType.CATEGORY,
                Payload = new CreateCategoryRequest { Name = "Electronics" },
                Reason = "New category",
                TargetId = 12
            };

            ApprovalJob testJob = new ApprovalJob
            {
                Id = 99,
                Type = baseRequest.Type,
                Operation = baseRequest.Operation,
                State = false,
                RequestedBy = 5,
                RequestedAt = DateTime.UtcNow,
                TargetId = baseRequest.TargetId,
                PayloadJson = JsonSerializer.Serialize(baseRequest.Payload),
                Reason = baseRequest.Reason
            };

            _approvalJobRepository.GetByIdTrackedAsync(jobId, ct).Returns(testJob);

            _unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>())
                .Returns(async ci =>
                {
                    var func = ci.Arg<Func<CancellationToken, Task>>();
                    await func(CancellationToken.None);
                });

            _approvalJobRepository.When(x => x.Update(Arg.Any<ApprovalJob>()))
                .Do(ci => job.State = true);

            // Act
            var result = await _sut.DecideAsync(jobId, adminUserId, dto, ct);

            // Assert            
            Assert.True(result.State);
            Assert.Equal(adminUserId, result.DecidedBy);
            Assert.Equal(dto.Reason, result.Reason);
            Assert.NotNull(result.DecidedAt);
        }

        [Fact]
        public async Task DecideAsync_ShouldRejectAndUpdateJob_WhenApproveIsFalse()
        {
            // Arrange
            int jobId = 1;
            int adminUserId = 20;
            var ct = CancellationToken.None;
            var job = new ApprovalJob { Id = jobId, RequestedBy = 5, Type = ApprovalJobType.PRODUCT };
            var dto = new DecideApprovalJobRequest { Approve = false, Reason = "Rejected due to inconsistency" };

            _approvalJobRepository.GetByIdTrackedAsync(jobId, ct).Returns(job);

            _unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>())
                .Returns(async ci =>
                {
                    var func = ci.Arg<Func<CancellationToken, Task>>();
                    await func(CancellationToken.None);
                });

            // Act
            var result = await _sut.DecideAsync(jobId, adminUserId, dto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(job.State);
            Assert.Equal(adminUserId, job.DecidedBy);
        }
    }
}
