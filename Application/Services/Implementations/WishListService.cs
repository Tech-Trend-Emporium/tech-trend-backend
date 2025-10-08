using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.WishList;
using General.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class WishListService : IWishListService
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WishListService(IWishListRepository wishListRepository, IProductRepository productRepository, ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _wishListRepository = wishListRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        private async Task<WishList> EnsureWishList(int userId, CancellationToken ct)
        {
            return await _wishListRepository.GetByUserIdAsync(userId, includeGraph: true, ct) ?? await _wishListRepository.CreateForUserAsync(userId, ct);
        }

        private async Task<WishList> Reload(WishList wl, CancellationToken ct)
        {
            return (await _wishListRepository.GetByUserIdAsync(wl.UserId, includeGraph: true, ct))!;
        }

        private async Task<Product> GetProductOr404Async(int productId, CancellationToken ct)
        {
            var p = await _productRepository.GetByIdAsync(ct, productId);
            if (p is null) throw new NotFoundException(ProductValidator.ProductNotFound(productId));

            return p;
        }

        public async Task<WishListResponse> GetAsync(int userId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);

            return WishListMapper.ToResponse(wl);
        }

        public async Task<WishListResponse> AddItemAsync(int userId, AddWishListItemRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var wl = await EnsureWishList(userId, ct);
            var product = await GetProductOr404Async(dto.ProductId, ct);

            wl.AddItem(product.Id);

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        public async Task<WishListResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            wl.RemoveItem(productId);

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        public async Task<WishListResponse> ClearAsync(int userId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            wl.Clear();

            await _unitOfWork.SaveChangesAsync(ct);
            wl = await Reload(wl, ct);

            return WishListMapper.ToResponse(wl);
        }

        public async Task MoveItemToCartAsync(int userId, int productId, CancellationToken ct = default)
        {
            var wl = await EnsureWishList(userId, ct);
            var item = wl.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;

            var cart = await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct) ?? await _cartRepository.CreateForUserAsync(userId, ct);

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is null) cart.Items.Add(CartMapper.ToEntity(cart.Id, productId, 1));

            wl.RemoveItem(productId);

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
