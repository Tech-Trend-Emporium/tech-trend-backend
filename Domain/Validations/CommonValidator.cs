using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public class CommonValidator
    {
        public static string CategoryCreationRequestedByEmployee(int employeeId) => $"Category creation requested by employee with ID #{employeeId}";
        public static string CategoryDeletionRequestedByEmployee(int employeeId, int categoryId) => $"Category deletion requested by employee with ID #{employeeId} for category with ID #{categoryId}";
        public static string ProductCreationRequestedByEmployee(int employeeId) => $"Product creation requested by employee with ID #{employeeId}";
        public static string ProductDeletionRequestedByEmployee(int employeeId, int productId) => $"Product deletion requested by employee with ID #{employeeId} for product with ID #{productId}";

        public const string CategoryCreationValidationMessage = "Category creation request has been submitted for approval.";
        public const string CategoryDeletionValidationMessage = "Category deletion request has been submitted for approval.";
        public const string ProductCreationValidationMessage = "Product creation request has been submitted for approval.";
        public const string ProductDeletionValidationMessage = "Product deletion request has been submitted for approval.";
    }
}
