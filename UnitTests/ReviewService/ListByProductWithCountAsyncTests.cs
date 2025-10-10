using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewService
{
    public class ListByProductWithCountAsyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut;

        public ListByProductWithCountAsyncTests()
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
        public async Task ListByProductWithCountAsync_ShouldReturnItemsAndTotal_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            int skip = 0, take = 50;
            var ct = CancellationToken.None;

            var reviews = new List<Review>
            {
                new Review { Id = 1, ProductId = productId, UserId = 10, Comment = "Great!" },
                new Review { Id = 2, ProductId = productId, UserId = 11, Comment = "Good!" }
            };

            var users = new List<User>
            {
                new User { Id = 10, Username = "Alice" },
                new User { Id = 11, Username = "Bob" }
            };

            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct)
                .Returns(true);

            _reviewRepository.ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), 0, 50, ct)
                .Returns(reviews);

            _reviewRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), ct)
                .Returns(reviews.Count);

            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>())
                .Returns(users);

            // Act
            var (result, total) = await _sut.ListByProductWithCountAsync(productId, skip, take, ct);

            // Assert
            Assert.Equal(reviews.Count, total);
            Assert.Equal(reviews.Count, result.Count);
            Assert.Contains(users[0].Username, result[0].Username);
            Assert.Contains(users[1].Username, result[1].Username);            

            await _productRepository.Received(1).ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct);
            await _reviewRepository.Received(1).ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), skip, take, ct);
            await _reviewRepository.Received(1).CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }

        [Fact]
        public async Task ListByProductWithCountAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999;
            int skip = 0, take = 50;
            var ct = CancellationToken.None;

            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct)
                .Returns(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.ListByProductWithCountAsync(productId, 0, 50, ct));

            Assert.Equal(ProductValidator.ProductNotFound(productId), ex.Message);

            await _reviewRepository.DidNotReceiveWithAnyArgs().ListAsync(default!, skip, take, ct);
            await _reviewRepository.DidNotReceiveWithAnyArgs().CountAsync(default!, ct);
        }

        [Fact]
        public async Task ListByProductWithCountAsync_ShouldReturnEmptyList_WhenNoReviewsExist()
        {
            // Arrange
            var productId = 1;
            int skip = 0, take = 50;
            var ct = CancellationToken.None;

            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct)
                .Returns(true);

            _reviewRepository.ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), skip, take, ct)
                .Returns(new List<Review>());

            _reviewRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), ct)
                .Returns(0);

            _userRepository.ListByIdsAsync(ct, Arg.Any<List<int>>())
                .Returns(new List<User>());

            // Act
            var result = await _sut.ListByProductWithCountAsync(productId, skip, take, ct);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }
    }
}
