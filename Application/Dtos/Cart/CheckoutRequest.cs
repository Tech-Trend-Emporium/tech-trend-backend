using Domain.Enums;
using Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.Cart
{
    public class CheckoutRequest : IValidatableObject
    {
        [Required(ErrorMessage = CartValidator.AddressRequiredMessage)]
        [StringLength(120, MinimumLength = 8, ErrorMessage = CartValidator.AddressLengthMessage)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = CartValidator.PaymentMethodRequiredMessage)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMethod PaymentMethod { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Address != null && Address != Address.Trim()) yield return new ValidationResult(CartValidator.AddressWhitespaceMessage, new[] { nameof(Address) });
            if (!Enum.IsDefined(typeof(PaymentMethod), PaymentMethod)) yield return new ValidationResult(CartValidator.PaymentMethodInvalidMessage, new[] { nameof(PaymentMethod) });
        }
    }
}
