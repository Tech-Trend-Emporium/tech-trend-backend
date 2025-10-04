using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class InventoryValidator
    {
        // Entity validation messages
        public const string AvailableGreaterThanTotalErrorMessage = "The field Available cannot be greater than Total field.";

        // DTO validation messages
        public const string TotalRequiredMessage = "Total is required.";
        public const string TotalNonNegativeMessage = "Total must be a non-negative integer.";
        public const string AvailableRequiredMessage = "Available is required.";
        public const string AvailableNonNegativeMessage = "Available must be a non-negative integer.";
        public const string ProductIdRequiredMessage = "ProductId is required.";
        public const string ProductIdPositiveMessage = "ProductId must be a positive integer.";
    }
}
