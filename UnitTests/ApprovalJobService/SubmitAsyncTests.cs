using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.ApprovalJob;
using Application.Exceptions;
using Application.Services;
using Application.Services.Implementations;
using Data.Entities;
using Domain.Enums;
using General.Dto.Category;
using General.Dto.Product;
using General.Mappers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ApprovalJobs
{
    public class SubmitAsyncTests
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly Application.Services.Implementations.ApprovalJobService _sut;

        public SubmitAsyncTests()
        {
            _approvalJobRepository = Substitute.For<IApprovalJobRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = Substitute.For<ICategoryService>();
            _productService = Substitute.For<IProductService>();

            _sut = new Application.Services.Implementations.ApprovalJobService(_approvalJobRepository, _unitOfWork, _categoryService, _productService);
        }

        [Fact]
        public async Task SubmitAsync_ShouldThrowArgumentNull_WhenDtoIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.SubmitAsync(1, null!, CancellationToken.None));
        }

        [Fact]
        public async Task SubmitAsync_ShouldThrowBadRequest_WhenDeleteWithoutTargetId()
        {
            // Arrange
            int requesterUserId = 1;
            var ct = CancellationToken.None;
            var dto = new SubmitApprovalJobRequest
            {
                Operation = Operation.DELETE,
                Type = ApprovalJobType.CATEGORY
            };
            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(
                () => _sut.SubmitAsync(requesterUserId, dto, ct));
        }

        [Fact]
        public async Task SubmitAsync_ShouldThrowBadRequest_WhenCreateWithoutPayload()
        {
            // Arrange
            int requesterUserId = 1;
            var ct = CancellationToken.None;
            var dto = new SubmitApprovalJobRequest
            {
                Operation = Operation.CREATE,
                Type = ApprovalJobType.CATEGORY,
                Payload = null
            };
            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(
                () => _sut.SubmitAsync(requesterUserId, dto, ct));

            Assert.Equal("Payload es obligatorio para CREATE.", ex.Message);
        }

        [Fact]
        public async Task SubmitAsync_ShouldThrowBadRequest_WhenPayloadTypeInvalid()
        {
            // Arrange
            int requesterUserId = 1;
            var ct = CancellationToken.None;
            var dto = new SubmitApprovalJobRequest
            {
                Operation = Operation.CREATE,
                Type = ApprovalJobType.CATEGORY,
                Payload = new CreateProductRequest() // Wrong payload type
            };

            await Assert.ThrowsAsync<BadRequestException>(
                () => _sut.SubmitAsync(requesterUserId, dto, ct));
        }

        [Fact]
        public async Task SubmitAsync_ShouldReturnApprovalJobResponse_WhenValidCreateCategory()
        {
            // Arrange
            int requesterUserId = 1;
            var ct = CancellationToken.None;
            var payload = new CreateCategoryRequest { Name = "Electronics" };

            var dto = new SubmitApprovalJobRequest
            {
                Operation = Operation.CREATE,
                Type = ApprovalJobType.CATEGORY,
                Payload = payload,
                Reason = "New category",
                TargetId = 12
            };

            ApprovalJob testJob = new ApprovalJob
            {
                Id = 99,
                Type = dto.Type,
                Operation = dto.Operation,
                State = false,
                RequestedBy = requesterUserId,
                RequestedAt = DateTime.UtcNow,
                TargetId = dto.TargetId,
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload),
                Reason = dto.Reason
            };

            var expectedJob = ApprovalJobMapper.ToResponse(testJob);

            _unitOfWork.SaveChangesAsync(ct).Returns(1);            

            // Act
            var result = await _sut.SubmitAsync(requesterUserId, dto, ct);

            // Assert
            Assert.NotNull(result);            
            Assert.Equal(expectedJob.Reason, result.Reason);
            Assert.Equal(expectedJob.Operation, result.Operation);
            Assert.Equal(expectedJob.Type, result.Type);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact]
        public async Task SubmitAsync_ShouldReturnApprovalJobResponse_WhenValidCreateProduct()
        {
            // Arrange
            int requesterUserId = 1;
            var ct = CancellationToken.None;
            var payload = new CreateProductRequest { Title = "SmarTV", Price = 450m, Description = "New tv" };

            var dto = new SubmitApprovalJobRequest
            {
                Operation = Operation.CREATE,
                Type = ApprovalJobType.PRODUCT,
                Payload = payload,
                Reason = "New product",
                TargetId = 12
            };

            ApprovalJob testJob = new ApprovalJob
            {
                Id = 99,
                Type = dto.Type,
                Operation = dto.Operation,
                State = false,
                RequestedBy = requesterUserId,
                RequestedAt = DateTime.UtcNow,
                TargetId = dto.TargetId,
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload),
                Reason = dto.Reason
            };

            var expectedJob = ApprovalJobMapper.ToResponse(testJob);

            _unitOfWork.SaveChangesAsync(ct).Returns(1);

            // Act
            var result = await _sut.SubmitAsync(requesterUserId, dto, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedJob.Reason, result.Reason);
            Assert.Equal(expectedJob.Operation, result.Operation);
            Assert.Equal(expectedJob.Type, result.Type);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }
    }
}
