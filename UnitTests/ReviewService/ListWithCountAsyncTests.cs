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
    public class ListWithCountAsyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut; // System Under Test

        public ListWithCountAsyncTests()
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
        public async Task ListWithCountAsync_ShouldReturnReviewsAndTotal_WhenDataExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            var reviews = new List<Review>
            {
                new() { Id = 1, UserId = 10, ProductId = 101, Rating = 5, Comment = "Excellent" },
                new() { Id = 2, UserId = 20, ProductId = 102, Rating = 4, Comment = "Good" },
                new() { Id = 3, UserId = 10, ProductId = 103, Rating = 3, Comment = "Okay" }
            };

            var users = new List<User>
            {
                new() { Id = 10, Username = "Alice" },
                new() { Id = 20, Username = "Bob" }
            };

            _reviewRepository.ListAsync(skip, take, ct).Returns(reviews);
            _reviewRepository.CountAsync(null, ct).Returns(reviews.Count);
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(users);

            // Act
            var result = await _sut.ListWithCountAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result.Items);
            Assert.Equal(reviews.Count, result.Total);
            Assert.Equal(reviews.Count, result.Items.Count);
            Assert.Equal(users[0].Username, result.Items[0].Username);


            await _reviewRepository.Received(1).ListAsync(0, 50, ct);
            await _reviewRepository.Received(1).CountAsync(null, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnEmpty_WhenNoReviewsExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            _reviewRepository.ListAsync(skip, take, ct).Returns(new List<Review>());
            _reviewRepository.CountAsync(null, ct).Returns(0);
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<User>());

            // Act
            var result = await _sut.ListWithCountAsync(skip, take, ct);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);

            await _reviewRepository.Received(1).ListAsync(skip, take, ct);
            await _reviewRepository.Received(1).CountAsync(null, ct);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldHandleMissingUsernamesGracefully()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            var reviews = new List<Review>
            {
                new() { Id = 1, UserId = 99, ProductId = 200, Rating = 5, Comment = "Great!" }
            };

            _reviewRepository.ListAsync(skip, take, ct).Returns(reviews);
            _reviewRepository.CountAsync(null, ct).Returns(1);
            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>()).Returns(new List<User>()); // No users found

            // Act
            var result = await _sut.ListWithCountAsync(skip, take, ct);

            // Assert
            Assert.Equal(reviews.Count, result.Total);
            Assert.Equal("Unknown", result.Items[0].Username);

            await _reviewRepository.Received(1).ListAsync(skip, take, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }
    }
}
