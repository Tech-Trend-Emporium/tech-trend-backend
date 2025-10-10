using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using General.Dto.Review;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewService
{
    public class UpdateasyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut; // System Under Test

        public UpdateasyncTests()
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
        public async Task UpdateAsync_ShouldUpdateReviewSuccessfully()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 1;

            var existingReview = new Review
            {
                Id = reviewId,
                UserId = 10,
                ProductId = 200,
                Rating = 3,
                Comment = "Old comment"
            };

            var user = new User { Id = 10, Username = "Alice" };

            var dto = new UpdateReviewRequest
            {
                Rating = 5,
                Comment = "Updated comment"
            };

            _reviewRepository.GetByIdAsync(ct, reviewId).Returns(existingReview);
            _userRepository.GetByIdAsync(ct, existingReview.UserId).Returns(user);

            // Act
            var result = await _sut.UpdateAsync(reviewId, dto, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(dto.Comment, result.Comment);
            Assert.Equal(dto.Rating, existingReview.Rating);

            _reviewRepository.Received(1).Update(existingReview);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 99;
            var dto = new UpdateReviewRequest
            {
                Rating = 5,
                Comment = "Updated comment"
            };

            var user = new User { Id = 10, Username = "Alice" };

            _reviewRepository.GetByIdAsync(ct, reviewId).Returns((Review?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(reviewId, dto, ct));
            
            await _reviewRepository.Received(1).GetByIdAsync(ct, reviewId);
            await _userRepository.DidNotReceive().GetByIdAsync( ct, user.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 1;

            var existingReview = new Review
            {
                Id = reviewId,
                UserId = 10,
                ProductId = 100,
                Rating = 3,
                Comment = "Comment"
            };

            _reviewRepository.GetByIdAsync(ct, reviewId).Returns(existingReview);
            _userRepository.GetByIdAsync(ct, existingReview.UserId).Returns((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(reviewId, new UpdateReviewRequest(), ct));

            await _reviewRepository.Received(1).GetByIdAsync(ct, reviewId);
            await _unitOfWork.DidNotReceive().SaveChangesAsync(ct);
        }
    }
}
