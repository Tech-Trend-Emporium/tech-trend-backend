using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Coupon
{
    public class UpdateCouponRequest : IValidatableObject
    {
        [Range(0, 100, ErrorMessage = CouponValidator.DiscountRangeMessage)]
        public decimal? Discount { get; set; }

        public bool? Active { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (ValidFrom.HasValue && ValidTo.HasValue && ValidTo < ValidFrom) yield return new ValidationResult(CouponValidator.ValidToAfterValidFromErrorMessage, new[] { nameof(ValidTo), nameof(ValidFrom) });
            if (ValidTo.HasValue && !ValidFrom.HasValue && ValidTo.Value < DateTime.UtcNow) yield return new ValidationResult(CouponValidator.ValidToInPastErrorMessage, new[] { nameof(ValidTo) });
        }
    }
}
