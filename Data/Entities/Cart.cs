using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(UserId))]
    public class Cart
    {
        public int Id { get; set; }

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    [Index(nameof(CartId), nameof(ProductId), IsUnique = true)]
    public class CartItem
    {
        public int Id { get; set; }

        [Range(1, 1000000)]
        public int Quantity { get; set; } = 1;

        [Required]
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }

}
