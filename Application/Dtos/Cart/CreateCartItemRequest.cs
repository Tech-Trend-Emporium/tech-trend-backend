using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Cart
{
    public class CreateCartItemRequest : IValidatableObject
    {
        [Required(ErrorMessage = CartValidator.ProductIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = CartValidator.ProductIdRangeMessage)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = CartValidator.QuantityRequiredMessage)]
        [Range(1, 100, ErrorMessage = CartValidator.QuantityRangeMessage)]
        public int Quantity { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Quantity <= 0) yield return new ValidationResult(CartValidator.QuantityPositiveMessage, new[] { nameof(Quantity) });
            if (ProductId <= 0) yield return new ValidationResult(CartValidator.ProductIdPositiveMessage, new[] { nameof(ProductId) });
        }
    }
}
