using Application.Dtos.WishList;
using Data.Entities;
using General.Dto.WishList;

namespace General.Mappers
{
    public static class WishListMapper
    {
        public static WishListResponse ToResponse(WishList entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            var items = entity.Items
                .OrderByDescending(i => i.AddedAt)
                .Select(i => new WishListItemResponse
                {
                    ProductId = i.ProductId,
                    Name = i.Product?.Title ?? string.Empty,
                    ImageUrl = i.Product?.ImageUrl ?? string.Empty,
                    UnitPrice = i.Product?.Price ?? 0m,
                    AddedAt = i.AddedAt
                })
                .ToList();

            return new WishListResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                Items = items
            };
        }
    }
}
