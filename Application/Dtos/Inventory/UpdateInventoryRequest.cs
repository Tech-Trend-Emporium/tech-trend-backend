using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace General.Dto.Inventory
{
    public class UpdateInventoryRequest : IValidatableObject
    {
        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.TotalNonNegativeMessage)]
        public int? Total { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = InventoryValidator.AvailableNonNegativeMessage)]
        public int? Available { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Total.HasValue && Available.HasValue && Available.Value > Total.Value)
            {
                yield return new ValidationResult(InventoryValidator.AvailableGreaterThanTotalErrorMessage, new[] { nameof(Available), nameof(Total) });
            }
        }
    }
}
