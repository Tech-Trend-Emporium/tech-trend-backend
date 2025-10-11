namespace Domain.Validations
{
    public class CouponValidator
    {
        // Service validation messages
        public static string CouponNotFound(int id) => $"The coupon with id '{id}' not found.";
        public static string CouponNotFound(string code) => $"The coupon with code '{code}' not found.";

        // Entity validation messages
        public const string ValidToAfterValidFromErrorMessage = "The field ValidTo must be after ValidFrom field.";
        public const string ValidToInPastErrorMessage = "The field ValidTo must be after current date.";

        // DTO validation messages
        public const string DiscountRequiredMessage = "The field Discount is required.";
        public const string DiscountRangeMessage = "The field Discount must be between 0.0 and 1.0.";
        public const string ValidFromRequiredMessage = "The field ValidFrom is required.";
        public const string DateRegex = @"^\d{4}-\d{2}-\d{2}$";
        public const string ValidFromFormatMessage = "The field ValidFrom must be in the format YYYY-MM-DD.";
        public const string ValidToFormatMessage = "The field ValidTo must be in the format  YYYY-MM-DD.";
    }
}
