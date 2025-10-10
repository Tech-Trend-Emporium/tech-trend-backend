using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewService
{
    public class ListAsyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut; // System Under Test

        public ListAsyncTests()
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
        public async Task ListAsync_ShouldReturnMappedReviews_WhenDataExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            var reviews = new List<Review>
            {
                new() { Id = 1, UserId = 10, ProductId = 101, Rating = 5, Comment = "Excellent!" },
                new() { Id = 2, UserId = 20, ProductId = 102, Rating = 4, Comment = "Good!" },
                new() { Id = 3, UserId = 10, ProductId = 103, Rating = 3, Comment = "Average" }
            };

            var users = new List<User>
            {
                new() { Id = 10, Username = "Alice" },
                new() { Id = 20, Username = "Bob" }
            };

            _reviewRepository.ListAsync(skip, take, ct).Returns(reviews);
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(users);

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviews.Count, result.Count);
            Assert.Contains(result, r => r.Username == "Alice");
            Assert.Contains(result, r => r.Username == "Bob");

            await _reviewRepository.Received(1).ListAsync(skip, take, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }

        [Fact]
        public async Task ListAsync_ShouldReturnEmptyList_WhenNoReviewsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            _reviewRepository.ListAsync(skip, take, ct).Returns(new List<Review>());
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<User>());

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _reviewRepository.Received(1).ListAsync(skip, take, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }

        [Fact]
        public async Task ListAsync_ShouldHandleMissingUsernamesGracefully()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0;
            int take = 50;

            var reviews = new List<Review>
            {
                new() { Id = 1, UserId = 100, ProductId = 999, Rating = 5, Comment = "Great!" }
            };

            _reviewRepository.ListAsync(skip, take, ct).Returns(reviews);
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<User>());

            // Act
            var result = await _sut.ListAsync(0, 50, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.NotNull(result.First().Username); // User not found case handled by mapper

            await _reviewRepository.Received(1).ListAsync(skip, take, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }
    }
}
