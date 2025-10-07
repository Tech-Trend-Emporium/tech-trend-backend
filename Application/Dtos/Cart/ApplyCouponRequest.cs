using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Cart
{
    public class ApplyCouponRequest : IValidatableObject
    {
        [Required(ErrorMessage = CartValidator.CouponCodeRequiredMessage)]
        [StringLength(36, MinimumLength = 12, ErrorMessage = CartValidator.CouponCodeLengthMessage)]
        [RegularExpression(CartValidator.CouponCodeRegex, ErrorMessage = CartValidator.CouponCodeFormatMessage)]
        public string CouponCode { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (CouponCode != null && CouponCode != CouponCode.Trim()) yield return new ValidationResult(CartValidator.CouponCodeWhitespaceMessage, new[] { nameof(CouponCode) });
        }
    }
}
