using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public class CouponValidator
    {
        // Service validation messages
        public static string CouponNotFound(int id) => $"The coupon with id '{id}' not found.";

        // Entity validation messages
        public const string ValidToAfterValidFromErrorMessage = "The field ValidTo must be after ValidFrom field.";
        public const string ValidToInPastErrorMessage = "The field ValidTo must be after current date.";

        // DTO validation messages
        public const string DiscountRequiredMessage = "Discount is required.";
        public const string DiscountRangeMessage = "Discount must be between 0 and 100.";
        public const string ValidFromRequiredMessage = "Valid from is required.";
    }
}
