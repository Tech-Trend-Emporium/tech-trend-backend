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
    public class RemoveItemAsyncTest
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly WishListService _sut; // System Under Test

        public RemoveItemAsyncTest()
        {
            _wishListRepository = Substitute.For<IWishListRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _cartRepository = Substitute.For<ICartRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _sut = new WishListService(_wishListRepository, _productRepository, _cartRepository, _unitOfWork);
        }

        [Fact]
        public async Task RemoveItemAsync_ShouldRemoveProduct_WhenExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var userId = 1;
            var productId = 101;
            var wl = new WishList { Id = 10, UserId = userId };
            wl.AddItem(productId);

            _wishListRepository.GetByUserIdAsync(userId, true, Arg.Any<CancellationToken>()).Returns(wl);            

            // Act
            var result = await _sut.RemoveItemAsync(userId, productId);

            // Assert
            Assert.Empty(result.Items);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact(DisplayName = "RemoveItemAsync does nothing if product not in wishlist")]
        public async Task RemoveItemAsync_ShouldDoNothing_WhenProductNotFoundInWishlist()
        {
            // Arrange
            var ct = CancellationToken.None;
            var userId = 2;
            var productId = 202;
            var wl = new WishList { Id = 20, UserId = userId };
            wl.AddItem(999); // different product

            _wishListRepository.GetByUserIdAsync(userId, true,ct).Returns(wl);            

            // Act
            var result = await _sut.RemoveItemAsync(userId, productId);

            // Assert
            
            Assert.DoesNotContain(result.Items, i => i.ProductId == productId);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact]
        public async Task RemoveItemAsync_ShouldCreateWishList_WhenNotExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            var userId = 3;
            var productId = 303;
            WishList newWishList = new WishList { Id = 30, UserId = userId };

            _wishListRepository.GetByUserIdAsync(userId, true,ct).Returns((WishList?)null, newWishList);
            _wishListRepository.CreateForUserAsync(userId, ct).Returns(newWishList);

            // Act
            var result = await _sut.RemoveItemAsync(userId, productId, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            await _wishListRepository.Received(1).CreateForUserAsync(userId, Arg.Any<CancellationToken>());
            await _wishListRepository.Received(2).GetByUserIdAsync(userId, true, ct);
        }
    }
}
