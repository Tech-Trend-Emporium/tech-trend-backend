using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Inventory
{
    public class CreateInventoryInlineRequest
    {
        [Required(ErrorMessage = InventoryValidator.TotalRequiredMessage)]
        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.TotalNonNegativeMessage)]
        public int Total { get; set; }

        [Required(ErrorMessage = InventoryValidator.AvailableRequiredMessage)]
        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.AvailableNonNegativeMessage)]
        public int Available { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Available > Total)
            {
                yield return new ValidationResult(InventoryValidator.AvailableGreaterThanTotalErrorMessage, new[] { nameof(Available), nameof(Total) });
            }
        }
    }
}
