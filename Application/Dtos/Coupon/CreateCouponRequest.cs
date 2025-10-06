using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Coupon
{
    public class CreateCouponRequest : IValidatableObject
    {
        [Required(ErrorMessage = CouponValidator.DiscountRequiredMessage)]
        [Range(0.0, 1.0, ErrorMessage = CouponValidator.DiscountRangeMessage)]
        public decimal Discount { get; set; }

        public bool Active { get; set; } = true;

        [Required(ErrorMessage = CouponValidator.ValidFromRequiredMessage)]
        [RegularExpression(CouponValidator.DateRegex, ErrorMessage = CouponValidator.ValidFromFormatMessage)]
        public DateTime ValidFrom { get; set; }

        [RegularExpression(CouponValidator.DateRegex, ErrorMessage = CouponValidator.ValidToFormatMessage)]
        public DateTime? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (ValidTo.HasValue && ValidTo < ValidFrom) yield return new ValidationResult(CouponValidator.ValidToAfterValidFromErrorMessage, new[] { nameof(ValidTo), nameof(ValidFrom) });
        }
    }
}
