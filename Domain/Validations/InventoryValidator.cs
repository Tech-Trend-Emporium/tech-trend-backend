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
    }
}
