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
    public class ListByProductAsyncTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut;

        public ListByProductAsyncTests()
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
        public async Task ListByProductAsync_ShouldReturnReviews_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            var reviews = new List<Review>
            {
                new Review { Id = 10, ProductId = productId, UserId = 1, Comment = "Great product" },
                new Review { Id = 11, ProductId = productId, UserId = 2, Comment = "Good quality" }
            };

            var users = new List<User>
            {
                new User { Id = 1, Username = "Alice" },
                new User { Id = 2, Username = "Bob" }
            };

            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct).Returns(Task.FromResult(true));

            _reviewRepository.ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), skip, take, ct).Returns(Task.FromResult((IReadOnlyList<Review>)reviews));

            _userRepository.ListByIdsAsync(ct, Arg.Is<List<int>>(ids => ids.SequenceEqual(new[] { 1, 2 }))).Returns(Task.FromResult((IReadOnlyList<User>)users));

            // Act
            var result = await _sut.ListByProductAsync(productId, skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviews.Count, result.Count);
            Assert.Equal(users[0].Username, result[0].Username);
            Assert.Equal(users[1].Username, result[1].Username);            

            await _productRepository.Received(1).ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct);
            await _reviewRepository.Received(1).ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), skip, take, ct);
            await _userRepository.Received(1).ListByIdsAsync(ct, Arg.Any<List<int>>());
        }

        [Fact]
        public async Task ListByProductAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 99;
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct).Returns(Task.FromResult(false));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.ListByProductAsync(productId, skip, take, ct));

            Assert.Equal(ProductValidator.ProductNotFound(productId), ex.Message);

            await _reviewRepository.DidNotReceive().ListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), skip, take, ct);
            await _userRepository.DidNotReceive().ListByIdsAsync(ct, Arg.Any<List<int>>());
        }
    }
}
