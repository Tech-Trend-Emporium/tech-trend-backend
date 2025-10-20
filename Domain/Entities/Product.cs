using Domain.Validations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Index(nameof(Title))]
    [Index(nameof(CategoryId))]
    public class Product : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required, Precision(18, 2)]
        [Range(0, 999999999999.99)]
        public decimal Price { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Url, MaxLength(2048)]
        public string? ImageUrl { get; set; }

        [Range(0, 5)]
        public double RatingRate { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int Count { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Inventory? Inventory { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishListItem> WishListItems { get; set; } = new List<WishListItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Count is < 0) yield return new ValidationResult(ProductValidator.CountNegativeErrorMessage, new[] { nameof(Count) });
        }
    }
}
