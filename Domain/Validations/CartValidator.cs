using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class CartValidator
    {
        // Entity validation messages
        public const string ProductAlreadyInCartErrorMessage = "The product is already in the cart.";

        // DTO validation messages
        public const string CouponCodeRequiredMessage = "Coupon code is required.";
        public const string CouponCodeLengthMessage = "Coupon code must be between 12 and 36 characters.";
        public const string CouponCodeRegex = @"^CPN\-(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{32})$";
        public const string CouponCodeFormatMessage = "Coupon code format is invalid.";
        public const string CouponCodeWhitespaceMessage = "Coupon code cannot start or end with spaces.";
        public const string QuantityRangeMessage = "Quantity must be between 1 and 100.";
        public const string QuantityRequiredMessage = "Quantity is required.";
        public const string QuantityPositiveMessage = "Quantity must be greater than zero.";
        public const string ProductIdRequiredMessage = "Product ID is required.";
        public const string ProductIdRangeMessage = "Product ID must be a positive integer.";
        public const string ProductIdPositiveMessage = "Product ID must be greater than zero.";
    }
}
