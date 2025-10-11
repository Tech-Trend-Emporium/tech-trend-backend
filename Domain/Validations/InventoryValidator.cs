namespace Domain.Validations
{
    public static class InventoryValidator
    {
        // Entity validation messages
        public const string AvailableGreaterThanTotalErrorMessage = "The field Available cannot be greater than Total field.";

        // DTO validation messages
        public const string TotalRequiredMessage = "The field Total is required.";
        public const string TotalNonNegativeMessage = "The field Total must be a non-negative integer.";
        public const string AvailableRequiredMessage = "The field Available is required.";
        public const string AvailableNonNegativeMessage = "The field Available must be a non-negative integer.";
        public const string ProductIdRequiredMessage = "The field ProductId is required.";
        public const string ProductIdPositiveMessage = "The field ProductId must be a positive integer.";
    }
}
