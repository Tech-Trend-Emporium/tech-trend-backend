using Application.Dtos.Cart;
using Data.Entities;
using General.Dto.Cart;

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
                    Name = i.Product?.Title ?? string.Empty,
                    ImageUrl = i.Product?.ImageUrl ?? string.Empty,
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

        public static OrderResponse ToOrderResponse(Cart cart)
        {
            if (cart is null) throw new ArgumentNullException(nameof(cart));

            var items = cart.Items.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                Name = i.Product?.Title ?? string.Empty,
                ImageUrl = i.Product?.ImageUrl ?? string.Empty,
                UnitPrice = i.Product?.Price ?? 0m,
                Quantity = i.Quantity,
                Subtotal = (i.Product?.Price ?? 0m) * i.Quantity
            }).ToList();

            return new OrderResponse
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Address = cart.Address,
                PaymentMethod = cart.PaymentMethod.ToString(),
                PaymentStatus = cart.PaymentStatus.ToString(),
                TotalAmount = cart.TotalAmount,
                PlacedAtUtc = cart.PlacedAtUtc,
                PaidAtUtc = cart.PaidAtUtc,
                Items = items
            };
        }

        public static IReadOnlyList<OrderResponse> ToOrderResponseList(IEnumerable<Cart> carts)
        {
            return carts.Select(ToOrderResponse).ToList();
        }
    }
}
