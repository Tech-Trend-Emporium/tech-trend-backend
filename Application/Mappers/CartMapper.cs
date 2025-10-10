using Application.Dtos.Cart;
using Data.Entities;
using General.Dto.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class CartMapper
    {
        public static CartResponse ToResponse(Cart entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            var items = entity.Items
                .OrderBy(i => i.Id)
                .Select(i => new CartItemResponse
                {
                    ProductId = i.ProductId,
                    UnitPrice = i.Product?.Price ?? 0m,
                    Quantity = i.Quantity,
                    LineTotal = (i.Product?.Price ?? 0m) * i.Quantity
                })
                .ToList();

            var subtotal = items.Sum(x => x.LineTotal);
            decimal? discountPct = entity.Coupon?.Discount;
            var discountAmount = discountPct.HasValue ? Math.Round(subtotal * discountPct.Value, 2) : 0m;
            var total = Math.Max(0, subtotal - discountAmount);

            return new CartResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Items = items,
                Subtotal = subtotal,
                Discount = discountPct,
                DiscountAmount = discountAmount,
                Total = total,
                CouponCode = entity.Coupon?.Code,
                CreatedAt = entity.CreatedAt
            };
        }

        public static CartItem ToEntity(AddCartItemRequest dto, int cartId)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new CartItem
            {
                CartId = cartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
        }

        public static CartItem ToEntity(int cartId, int productId, int quantity)
        {
            if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (cartId <= 0) throw new ArgumentOutOfRangeException(nameof(cartId));

            return new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity
            };
        }
    }
}
