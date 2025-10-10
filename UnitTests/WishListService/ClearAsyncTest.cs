using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.WishListServices
{
    public class ClearAsyncTests
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WishListService _service;

        public ClearAsyncTests()
        {
            _wishListRepository = Substitute.For<IWishListRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _cartRepository = Substitute.For<ICartRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _service = new WishListService(
                _wishListRepository,
                _productRepository,
                _cartRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task ClearAsync_ShouldClearWishListAndReturnResponse_WhenWishListExists()
        {
            // Arrange
            int userId = 1;
            var ct = CancellationToken.None;

            var existingWishList = new WishList { 
                Id = 1, 
                UserId = userId, 
                Items = new List<WishListItem> {
                    new WishListItem { Id = 1, ProductId = 10 },
                    new WishListItem { Id = 2, ProductId = 20 }
                }
            };

            var clearedWishList = new WishList { Id = 1, UserId = userId, Items = new List<WishListItem>() };

            _wishListRepository.GetByUserIdAsync(userId, includeGraph: true, ct)
                .Returns(existingWishList);

            _unitOfWork.SaveChangesAsync(ct).Returns(1);
            

            // Act
            var result = await _service.ClearAsync(userId, ct);

            // Assert
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
            Assert.Empty(result.Items);
            Assert.Equal(userId, result.UserId);
        }
        
        [Fact]
        public async Task ClearAsync_ShouldCreateNewWishList_WhenNoneExists()
        {
            // Arrange
            int userId = 99;
            var ct = CancellationToken.None;

            var createdWishList = new WishList { Id = 5, UserId = userId, Items = new List<WishListItem>() };

            _wishListRepository.GetByUserIdAsync(userId, includeGraph: true, ct).Returns((WishList?)null, createdWishList);

            _wishListRepository.CreateForUserAsync(userId, ct).Returns(createdWishList);

            _unitOfWork.SaveChangesAsync(ct).Returns(1);


            // Act
            var result = await _service.ClearAsync(userId, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(userId, result.UserId);
        }       
    }
        
}
