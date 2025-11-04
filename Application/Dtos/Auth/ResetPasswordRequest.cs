using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class ResetPasswordRequest : IValidatableObject
    {
        [Required(ErrorMessage = AuthValidator.ResetTokenRequiredMessage)]
        [StringLength(2048, ErrorMessage = AuthValidator.ResetTokenLengthMessage)]
        [RegularExpression(AuthValidator.ResetTokenRegex, ErrorMessage = AuthValidator.ResetTokenFormatMessage)]
        public string ResetToken { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.PasswordRequiredMessage)]
        [StringLength(AuthValidator.PasswordMaxLength, MinimumLength = AuthValidator.PasswordMinLength, ErrorMessage = AuthValidator.PasswordLengthMessage)]
        [RegularExpression(AuthValidator.PasswordRegex, ErrorMessage = AuthValidator.PasswordRegexMessage)]
        public string NewPassword { get; set; } = null!;

        [Compare(nameof(NewPassword), ErrorMessage = AuthValidator.PasswordsMustMatchMessage)]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (ResetToken != null && ResetToken != ResetToken.Trim()) yield return new ValidationResult(AuthValidator.ResetTokenWhitespaceMessage, new[] { nameof(ResetToken) });
        }
    }
}
