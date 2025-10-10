using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.Review;
using General.Mappers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ReviewService
{
    public class ReviewServiceTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Services.Implementations.ReviewService _sut; // System Under Test

        public ReviewServiceTests()
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
        public async Task CreateAsync_ShouldCreateReview_WhenDataIsValid()
        {
            // Arrange
            var ct = CancellationToken.None;
            var reviewId = 1;
            var dto = new CreateReviewRequest
            {
                Username = "TestUser",
                ProductId = 10,
                Rating = 5,
                Comment = "Excellent product!"
            };

            var user = new User { Id = 1, Username = dto.Username };
            var reviewEntity = ReviewMapper.ToEntity(dto, user.Id);
            reviewEntity.Id = reviewId;
            var expectedReview = ReviewMapper.ToResponse(reviewEntity,user.Username);

            _userRepository.GetAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct : ct).Returns(user);
            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct).Returns(true);
            _reviewRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), ct).Returns(false);           


            // Act
            var result = await _sut.CreateAsync(dto, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReview.Id, result.Id);
            Assert.Equal(expectedReview.Username, result.Username);
            Assert.Equal(expectedReview.Comment, result.Comment);                        
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new CreateReviewRequest { Username = "UnknownUser", ProductId = 1 };

            _userRepository.GetAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct: ct).Returns((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(dto, ct));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new CreateReviewRequest { Username = "User1", ProductId = 10 };

            var user = new User { Id = 1, Username = "User1" };

            _userRepository.GetAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct: ct).Returns(user);
            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateAsync(dto, ct));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowConflictException_WhenDuplicateReviewExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var dto = new CreateReviewRequest { Username = "User1", ProductId = 10 };

            var user = new User { Id = 1, Username = "User1" };

            _userRepository.GetAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct: ct).Returns(user);
            _productRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Product, bool>>>(), ct).Returns(true);
            _reviewRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>(), ct).Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateAsync(dto, ct));            
        }
    }
}
