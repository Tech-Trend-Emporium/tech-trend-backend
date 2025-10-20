using Domain.Validations;
using System.ComponentModel.DataAnnotations;

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
