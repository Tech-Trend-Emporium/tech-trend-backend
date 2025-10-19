using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace General.Dto.Inventory
{
    public class CreateInventoryRequest : IValidatableObject
    {
        [Required(ErrorMessage = InventoryValidator.TotalRequiredMessage)]
        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.TotalNonNegativeMessage)]
        public int Total { get; set; }

        [Required(ErrorMessage = InventoryValidator.AvailableRequiredMessage)]
        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.AvailableNonNegativeMessage)]
        public int Available { get; set; }

        [Required(ErrorMessage = InventoryValidator.ProductIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = InventoryValidator.ProductIdPositiveMessage)]
        public int ProductId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Available > Total)
            {
                yield return new ValidationResult(InventoryValidator.AvailableGreaterThanTotalErrorMessage, new[] { nameof(Available), nameof(Total) });
            }
        }
    }
}
