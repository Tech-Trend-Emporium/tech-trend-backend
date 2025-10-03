using Domain.Validations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Coupon : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string Code { get; set; } = null!;

        [Required, Precision(18, 2)]
        [Range(0, 100)]
        public decimal Discount { get; set; } 

        [Required]
        public bool Active { get; set; } = true;

        [Required]
        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (ValidTo.HasValue && ValidTo < ValidFrom) yield return new ValidationResult(CouponValidator.ValidToAfterValidFromErrorMessage, new[] { nameof(ValidTo), nameof(ValidFrom) });
        }
    }
}
