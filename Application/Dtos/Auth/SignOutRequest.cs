using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class SignOutRequest : IValidatableObject
    {
        [StringLength(AuthValidator.RefreshTokenMaxLength, MinimumLength = AuthValidator.RefreshTokenMinLength, ErrorMessage = AuthValidator.RefreshTokenLengthMessage)]
        public string? RefreshToken { get; set; }

        [Required(ErrorMessage = AuthValidator.AllSessionsRequiredMessage)]
        public bool AllSessions { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AllSessions && string.IsNullOrWhiteSpace(RefreshToken))
            {
                yield return new ValidationResult(AuthValidator.RefreshTokenRequiredForSingleLogoutMessage, new[] { nameof(RefreshToken) });
            }
        }
    }
}
