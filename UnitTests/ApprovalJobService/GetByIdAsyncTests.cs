using Application.Abstraction;
using Application.Abstractions;
using Application.Services;
using Data.Entities;
using Domain.Enums;
using General.Dto.ApprovalJob;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ApprovalJobService
{
    public class ApprovalServiceGetByIdTests
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly Application.Services.Implementations.ApprovalJobService _sut;

        public ApprovalServiceGetByIdTests()
        {
            _approvalJobRepository = Substitute.For<IApprovalJobRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = Substitute.For<ICategoryService>();
            _productService = Substitute.For<IProductService>();

            _sut = new Application.Services.Implementations.ApprovalJobService(_approvalJobRepository, _unitOfWork, _categoryService, _productService);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedResponse_WhenJobExists()
        {
            // Arrange
            var job = new ApprovalJob
            {
                Id = 1,
                Type = ApprovalJobType.PRODUCT,
                RequestedBy = 5,
                RequestedAt = DateTime.UtcNow,
                Operation = Operation.CREATE,
                State = false
            };

            var expectedResponse = new ApprovalJobResponse
            {
                Id = job.Id,
                Type = job.Type.ToString(),
                RequestedBy = job.RequestedBy
            };

            _approvalJobRepository.GetByIdAsync(Arg.Any<CancellationToken>(), job.Id)
                .Returns(job);

            // Act
            var result = await _sut.GetByIdAsync(job.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result!.Id);
            Assert.Equal(expectedResponse.Type, result.Type);
            Assert.Equal(expectedResponse.RequestedBy, result.RequestedBy);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenJobDoesNotExist()
        {
            // Arrange
            _approvalJobRepository.GetByIdAsync(Arg.Any<CancellationToken>(), Arg.Any<int>())
                .Returns((ApprovalJob?)null);

            // Act
            var result = await _sut.GetByIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }      
    }
}
