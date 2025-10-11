using Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
        public string ValidFrom { get; set; } = null!;

        [RegularExpression(CouponValidator.DateRegex, ErrorMessage = CouponValidator.ValidToFormatMessage)]
        public string? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (!DateTime.TryParseExact(ValidFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate)) yield return new ValidationResult(CouponValidator.ValidFromFormatMessage, new[] { nameof(ValidFrom) });

            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(ValidTo))
            {
                if (DateTime.TryParseExact(ValidTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo)) toDate = parsedTo;
                else yield return new ValidationResult(CouponValidator.ValidToFormatMessage, new[] { nameof(ValidTo) });
            }

            if (toDate.HasValue && toDate < fromDate) yield return new ValidationResult(CouponValidator.ValidToAfterValidFromErrorMessage, new[] { nameof(ValidTo), nameof(ValidFrom) });
        }

        public (DateTime ValidFromDate, DateTime? ValidToDate) ToDateTimes()
        {
            DateTime from = DateTime.ParseExact(ValidFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime? to = string.IsNullOrWhiteSpace(ValidTo) ? null : DateTime.ParseExact(ValidTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            return (from, to);
        }
    }
}
