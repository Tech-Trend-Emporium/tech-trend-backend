using Application.Abstraction;
using Application.Abstractions;
using Application.Dtos.Cart;
using Application.Exceptions;
using Data.Entities;
using General.Dto.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<CartResponse> AddItemAsync(int userId, CreateCartItemRequest dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> ApplyCouponAsync(int userId, ApplyCouponRequest dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CheckoutAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> GetAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        private async Task<Cart> EnsureCart(int userId, CancellationToken ct)
        {
            return await _cartRepository.GetByUserIdAsync(userId, includeGraph: true, ct) ?? await _cartRepository.CreateForUserAsync(userId, ct);
        }

        private async Task<Cart> Reload(Cart cart, CancellationToken ct)
        {
            return (await _cartRepository.GetByUserIdAsync(cart.UserId, includeGraph: true, ct))!;
        }

        private static void ValidateStock(CartItem item, Product product)
        {
            if (item.Quantity < 1) throw new BadRequestException("La cantidad debe ser al menos 1.");
            if (item.Quantity > product.Count) throw new ConflictException($"No hay stock suficiente de '{product.Title}'.");
        }
    }
}
