using General.Dto.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IWishListService
    {
        Task<WishListResponse> GetAsync(int userId, CancellationToken ct = default);
        Task<WishListResponse> AddItemAsync(int userId, AddWishListItemRequest dto, CancellationToken ct = default);
        Task<WishListResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);
        Task<WishListResponse> ClearAsync(int userId, CancellationToken ct = default);
        Task MoveItemToCartAsync(int userId, int productId, CancellationToken ct = default);
    }
}
