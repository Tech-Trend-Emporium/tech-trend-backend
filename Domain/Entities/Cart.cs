using Domain.Enums;
using Domain.Validations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Index(nameof(UserId), IsUnique = false)]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [MaxLength(120)]
        public string? Address { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        [Required]
        public CartStatus Status { get; set; } = CartStatus.ACTIVE;

        [Precision(18, 2)]
        public decimal? TotalAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PlacedAtUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(int productId)
        {
            if (Items.Any(i => i.ProductId == productId))
                throw new InvalidOperationException(CartValidator.ProductAlreadyInCartErrorMessage);

            Items.Add(new CartItem { ProductId = productId, Cart = this });
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;
            Items.Remove(item);
        }

        public void Clear() => Items.Clear();
    }

    [Index(nameof(CartId), nameof(ProductId), IsUnique = true)]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
