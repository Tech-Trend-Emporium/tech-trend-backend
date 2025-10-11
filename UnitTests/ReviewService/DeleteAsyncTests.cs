using Application.Abstraction;
using Application.Abstractions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewService
{
    public class DeleteAsyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut; // System Under Test

        public DeleteAsyncTests()
        {
            _reviewRepository = Substitute.For<IReviewRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _sut = new Application.Services.Implementations.ReviewService(
                _reviewRepository,
                _userRepository,
                _productRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenReviewIsDeleted()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 1;

            _reviewRepository.DeleteByIdAsync(ct, reviewId).Returns(true);

            // Act
            var result = await _sut.DeleteAsync(reviewId, ct);

            // Assert
            Assert.True(result);
            await _reviewRepository.Received(1).DeleteByIdAsync(ct, reviewId);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenReviewDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 100;

            _reviewRepository.DeleteByIdAsync(ct, reviewId).Returns(false);

            // Act
            var result = await _sut.DeleteAsync(reviewId, ct);

            // Assert
            Assert.False(result);
            await _reviewRepository.Received(1).DeleteByIdAsync(ct, reviewId);
            await _unitOfWork.DidNotReceive().SaveChangesAsync(ct);
        }
    }
}
