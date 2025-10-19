namespace Domain.Validations
{
    public static class CartValidator
    {
        // Entity validation messages
        public const string ProductAlreadyInCartErrorMessage = "The product is already in the cart.";

        // Service validation messages
        public const string QuantityMinErrorMessage = "The field Quantity must be at least 1.";
        public static string InventoryInvalid(string productTitle) => $"Invalid inventory for '{productTitle}'.";
        public static string InventoryInsufficient(string productTitle, int available) => $"Insufficient inventory for '{productTitle}'. Available stock is '{available}'";
        public static string InventoryNotConfigured(int productId) => $"Inventory for product '{productId}' is not configured.";
        public static string InventoryNotConfiguredForProduct(string productTitle) => $"Inventory for product '{productTitle}' is not configured.";

        // DTO validation messages
        public const string CouponCodeRequiredMessage = "The field CouponCode is required.";
        public const string CouponCodeLengthMessage = "The field CouponCode must be between 12 and 36 characters.";
        public const string CouponCodeRegex = @"^CPN\-(?:[A-Fa-f0-9]{8}|[A-Fa-f0-9]{32})$";
        public const string CouponCodeFormatMessage = "The field CouponCode format is invalid.";
        public const string CouponCodeWhitespaceMessage = "The field CouponCode cannot start or end with spaces.";
        public const string QuantityRangeMessage = "The field Quantity must be between 1 and 100.";
        public const string QuantityRequiredMessage = "The field Quantity is required.";
        public const string QuantityPositiveMessage = "The field Quantity must be greater than zero.";
        public const string ProductIdRequiredMessage = "The field ProductId is required.";
        public const string ProductIdRangeMessage = "The field ProductId must be a positive integer.";
        public const string ProductIdPositiveMessage = "The field ProductId must be greater than zero.";
        public const string ProductNotInCartMessage = "The product is not in the cart.";
        public const string CouponNotActiveMessage = "The coupon is not active.";
        public const string CouponExpiredMessage = "The coupon has expired.";
    }
}
