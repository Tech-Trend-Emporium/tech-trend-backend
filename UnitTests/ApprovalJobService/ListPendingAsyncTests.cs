using Application.Abstraction;
using Application.Abstractions;
using Application.Services;
using Data.Entities;
using Domain.Enums;
using General.Dto.ApprovalJob;
using General.Dto.Category;
using General.Mappers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests.ApprovalJobService
{
    public class ListPendingAsyncTests
    {
        private readonly IApprovalJobRepository _approvalJobRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly Application.Services.Implementations.ApprovalJobService _sut;

        public ListPendingAsyncTests()
        {
            _approvalJobRepository = Substitute.For<IApprovalJobRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _categoryService = Substitute.For<ICategoryService>();
            _productService = Substitute.For<IProductService>();

            _sut = new Application.Services.Implementations.ApprovalJobService(_approvalJobRepository, _unitOfWork, _categoryService, _productService);
        }

        [Fact]
        public async Task ListPendingAsync_ShouldReturnMappedResponses_WhenJobsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var jobs = new List<ApprovalJob>
            {
                new() { Id = 1, Type = ApprovalJobType.CATEGORY, RequestedBy = 10, Reason = "Reason1", PayloadJson = JsonSerializer.Serialize(new CreateCategoryRequest { Name = "Electronics" }) },
                new() { Id = 2, Type = ApprovalJobType.PRODUCT, RequestedBy = 20, Reason = "Reason2", PayloadJson = JsonSerializer.Serialize(new CreateCategoryRequest { Name = "Pets" }) }
            };

            _approvalJobRepository.ListPendingAsync(skip, take, ct).Returns(jobs);

            var expectedResponses = ApprovalJobMapper.ToResponseList(jobs);            

            // Act
            var result = await _sut.ListPendingAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(jobs.Count, result.Count);
            Assert.Collection(result,
                item => Assert.Equal(1, item.Id),
                item => Assert.Equal(2, item.Id));
        }

        [Fact]
        public async Task ListPendingAsync_ShouldReturnEmptyList_WhenNoJobsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            _approvalJobRepository.ListPendingAsync(skip, take, ct).Returns(new List<ApprovalJob>());

            // Act
            var result = await _sut.ListPendingAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
