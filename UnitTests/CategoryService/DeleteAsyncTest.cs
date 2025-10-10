using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
//Code generated with ChatGPT
namespace UnitTests.CategoryServices
{    

    public class DeleteAsyncTest
    {
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly CategoryService _sut;

        public DeleteAsyncTest()
        {
            _sut = new CategoryService(_categoryRepository, _unitOfWork);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenCategoryIsDeleted()
        {
            // Arrange
            var id = 1;
            var ct = CancellationToken.None;

            _categoryRepository.DeleteByIdAsync(ct, id).Returns(true);

            // Act
            var result = await _sut.DeleteAsync(id, ct);

            // Assert
            Assert.True(result);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
            await _categoryRepository.Received(1).DeleteByIdAsync(ct, id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCategoryNotDeleted()
        {
            // Arrange
            var id = 99;
            var ct = CancellationToken.None;

            _categoryRepository.DeleteByIdAsync(ct, id).Returns(false);

            // Act
            var result = await _sut.DeleteAsync(id, ct);

            // Assert
            Assert.False(result);
            await _unitOfWork.DidNotReceive().SaveChangesAsync(ct);
            await _categoryRepository.Received(1).DeleteByIdAsync(ct, id);
        }
    }

}
