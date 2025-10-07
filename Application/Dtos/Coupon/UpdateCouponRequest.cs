using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Coupon
{
    public class UpdateCouponRequest : IValidatableObject
    {
        [Range(0.0, 1.0, ErrorMessage = CouponValidator.DiscountRangeMessage)]
        public decimal? Discount { get; set; }

        public bool? Active { get; set; }

        [RegularExpression(CouponValidator.DateRegex, ErrorMessage = CouponValidator.ValidFromFormatMessage)]
        public string? ValidFrom { get; set; }

        [RegularExpression(CouponValidator.DateRegex, ErrorMessage = CouponValidator.ValidToFormatMessage)]
        public string? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (!string.IsNullOrWhiteSpace(ValidFrom))
            {
                if (DateTime.TryParseExact(ValidFrom.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom)) fromDate = parsedFrom;
                else yield return new ValidationResult(CouponValidator.ValidFromFormatMessage, new[] { nameof(ValidFrom) });
            }

            if (!string.IsNullOrWhiteSpace(ValidTo))
            {
                if (DateTime.TryParseExact(ValidTo.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo)) toDate = parsedTo;
                else yield return new ValidationResult(CouponValidator.ValidToFormatMessage, new[] { nameof(ValidTo) });
            }

            if (fromDate.HasValue && toDate.HasValue && toDate.Value < fromDate.Value) yield return new ValidationResult(CouponValidator.ValidToAfterValidFromErrorMessage, new[] { nameof(ValidTo), nameof(ValidFrom) });
            if (!fromDate.HasValue && toDate.HasValue && toDate.Value < DateTime.UtcNow.Date) yield return new ValidationResult(CouponValidator.ValidToInPastErrorMessage, new[] { nameof(ValidTo) });
        }

        public (DateTime? ValidFromDate, DateTime? ValidToDate) ToDateTimes()
        {
            DateTime? from = string.IsNullOrWhiteSpace(ValidFrom) ? null : DateTime.ParseExact(ValidFrom.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime? to = string.IsNullOrWhiteSpace(ValidTo) ? null : DateTime.ParseExact(ValidTo.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

            return (from, to);
        }
    }
}
