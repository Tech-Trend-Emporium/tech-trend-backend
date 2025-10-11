using Application.Dtos.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Cart
{
    public class CartResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? Total { get; set; }
        public string? CouponCode { get; set; }
        public IReadOnlyList<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public DateTime CreatedAt { get; set; }
    }
}
