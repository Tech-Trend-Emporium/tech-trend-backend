using Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Dtos.Auth
{
    public class SignUpRequest : IValidatableObject
    {
        [Required(ErrorMessage = AuthValidator.UsernameRequiredMessage)]
        [StringLength(AuthValidator.UsernameMaxLength, MinimumLength = AuthValidator.UsernameMinLength, ErrorMessage = AuthValidator.UsernameLengthMessage)]
        [RegularExpression(AuthValidator.UsernameRegex, ErrorMessage = AuthValidator.UsernameRegexMessage)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.EmailRequiredMessage)]
        [EmailAddress(ErrorMessage = AuthValidator.EmailInvalidMessage)]
        [StringLength(AuthValidator.EmailMaxLength, ErrorMessage = AuthValidator.EmailMaxLengthMessage)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.PasswordRequiredMessage)]
        [StringLength(AuthValidator.PasswordMaxLength, MinimumLength = AuthValidator.PasswordMinLength, ErrorMessage = AuthValidator.PasswordLengthMessage)]
        [RegularExpression(AuthValidator.PasswordRegex, ErrorMessage = AuthValidator.PasswordRegexMessage)]
        public string Password { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = AuthValidator.QuestionIdRangeMessage)]
        public int? RecoveryQuestionId { get; set; }

        [StringLength(2000, MinimumLength = 1, ErrorMessage = AuthValidator.AnswerLengthMessage)]
        public string? RecoveryAnswer { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var username = Username ?? string.Empty;
            var email = Email ?? string.Empty;
            var password = Password ?? string.Empty;

            if (Username != null && (Username != Username.Trim())) yield return new ValidationResult( AuthValidator.UsernameNoLeadingTrailingSpacesMessage, new[] { nameof(Username) });
            if (Email != null && (Email != Email.Trim())) yield return new ValidationResult( AuthValidator.EmailNoLeadingTrailingSpacesMessage, new[] { nameof(Email) });
            if (Password != null && (Password != Password.Trim())) yield return new ValidationResult( AuthValidator.PasswordNoLeadingTrailingSpacesMessage, new[] { nameof(Password) });
            
            if (Regex.IsMatch(username, AuthValidator.UsernameNoConsecutivePunctRegex)) yield return new ValidationResult( AuthValidator.UsernameNoConsecutivePunctMessage, new[] { nameof(Username) });
            if (Regex.IsMatch(username, AuthValidator.UsernameCannotBeEmailRegex)) yield return new ValidationResult( AuthValidator.UsernameCannotBeEmailMessage, new[] { nameof(Username) });

            var emailLocal = email.Split('@')[0];
            if (!string.IsNullOrWhiteSpace(username) && password.IndexOf(username, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                yield return new ValidationResult(AuthValidator.PasswordNotContainUsernameMessage, new[] { nameof(Password) });
            }

            if (!string.IsNullOrWhiteSpace(emailLocal) && password.IndexOf(emailLocal, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                yield return new ValidationResult(AuthValidator.PasswordNotContainEmailLocalMessage, new[] { nameof(Password) });
            }

            if (!string.IsNullOrWhiteSpace(password) && (string.Equals(password, username, StringComparison.OrdinalIgnoreCase) || string.Equals(password, email, StringComparison.OrdinalIgnoreCase)))
            {
                yield return new ValidationResult(AuthValidator.PasswordNotEqualUsernameOrEmailMessage, new[] { nameof(Password) });
            }

            var hasQ = RecoveryQuestionId.HasValue;
            var hasA = !string.IsNullOrWhiteSpace(RecoveryAnswer);

            if (hasQ && !hasA) yield return new ValidationResult(AuthValidator.AnswerRequiredWhenQuestionProvidedMessage, new[] { nameof(RecoveryAnswer) });
            if (!hasQ && hasA) yield return new ValidationResult(AuthValidator.QuestionIdRequiredWhenAnswerProvidedMessage, new[] { nameof(RecoveryQuestionId) });
            if (hasA && RecoveryAnswer != RecoveryAnswer!.Trim()) yield return new ValidationResult(AuthValidator.AnswerWhitespaceMessage, new[] { nameof(RecoveryAnswer) });
            if (hasQ && RecoveryQuestionId!.Value <= 0) yield return new ValidationResult(AuthValidator.QuestionIdPositiveMessage, new[] { nameof(RecoveryQuestionId) });
        }
    }
}
