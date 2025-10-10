using Application.Abstraction;
using Application.Abstractions;
using Application.Services.Implementations;
using Data.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.WishListServices
{
    public class WishListServiceTests
    {
        private readonly IWishListRepository _wishListRepository = Substitute.For<IWishListRepository>();
        private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
        private readonly ICartRepository _cartRepository = Substitute.For<ICartRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly WishListService _sut; // system under test

        public WishListServiceTests()
        {
            
            _sut = new WishListService(_wishListRepository, _productRepository, _cartRepository, _unitOfWork);
        }
        /*
        [Fact]
        public async Task EnsureWishList_ReturnsExistingWishList_IfExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var userId = 5;
            var existingWishList = new WishList 
            { 
                Id = 10, 
                UserId = userId,
                Items = new List<WishListItem>
                {
                    new WishListItem { Id = 1, ProductId = 100, WishListId = 10 },
                    new WishListItem { Id = 2, ProductId = 200, WishListId = 10 }
                }
            };
            _wishListRepository.GetByUserIdAsync(userId, true, ct).Returns(existingWishList);

            // Act
            var result = await _sut.EnsureWishList(userId, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingWishList.Id, result.Id);
            await _wishListRepository.DidNotReceive().CreateForUserAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task EnsureWishList_CreatesNewWishList_IfNoneExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var userId = 7;
            var newWishList = new WishList
            {
                Id = 11,
                UserId = userId,
                Items = new List<WishListItem>
                {
                    new WishListItem { Id = 1, ProductId = 100, WishListId = 10 },
                    new WishListItem { Id = 2, ProductId = 200, WishListId = 10 }
                }
            };           

            _wishListRepository.GetByUserIdAsync(userId, true, ct).Returns((WishList?)null);
            _wishListRepository.CreateForUserAsync(userId, ct).Returns(newWishList);

            // Act
            var result = await _sut.EnsureWishList(userId, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newWishList.Id, result.Id);
            await _wishListRepository.Received(1).CreateForUserAsync(userId, Arg.Any<CancellationToken>());
            await _wishListRepository.Received(1).GetByUserIdAsync(userId, true, ct);
        }
        */
    }
}
