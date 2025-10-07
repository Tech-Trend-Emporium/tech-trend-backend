using Application.Dtos.Cart;
using General.Dto.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICartService
    {
        Task<CartResponse> GetAsync(int userId, CancellationToken ct = default);
        Task<CartResponse> AddItemAsync(int userId, CreateCartItemRequest dto, CancellationToken ct = default);
        Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest dto, CancellationToken ct = default);
        Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);
        Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default);
        Task<CartResponse> ApplyCouponAsync(int userId, ApplyCouponRequest dto, CancellationToken ct = default);
        Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default);
        Task CheckoutAsync(int userId, CancellationToken ct = default);
    }
}
