using Domain.Enums;
using Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace General.Dto.User
{
    public class CreateUserRequest : IValidatableObject
    {
        [Required(ErrorMessage = UserValidator.UsernameRequiredMessage)]
        [StringLength(UserValidator.UsernameMaxLength, MinimumLength = UserValidator.UsernameMinLength, ErrorMessage = UserValidator.UsernameLengthMessage)]
        [RegularExpression(UserValidator.UsernameRegex, ErrorMessage = UserValidator.UsernameRegexMessage)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = UserValidator.EmailRequiredMessage)]
        [EmailAddress(ErrorMessage = UserValidator.EmailInvalidMessage)]
        [StringLength(UserValidator.EmailMaxLength, ErrorMessage = UserValidator.EmailMaxLengthMessage)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = UserValidator.PasswordRequiredMessage)]
        [StringLength(UserValidator.PasswordMaxLength, MinimumLength = UserValidator.PasswordMinLength, ErrorMessage = UserValidator.PasswordLengthMessage)]
        [RegularExpression(UserValidator.PasswordRegex, ErrorMessage = UserValidator.PasswordRegexMessage)]
        public string Password { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; } = Role.SHOPPER;

        public bool IsActive { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            var username = Username ?? string.Empty;
            var email = Email ?? string.Empty;
            var password = Password ?? string.Empty;

            if (Username != null && (Username != Username.Trim())) yield return new ValidationResult(UserValidator.UsernameNoLeadingTrailingSpacesMessage, new[] { nameof(Username) });
            if (Email != null && (Email != Email.Trim())) yield return new ValidationResult(UserValidator.EmailNoLeadingTrailingSpacesMessage, new[] { nameof(Email) });
            if (Password != null && (Password != Password.Trim())) yield return new ValidationResult(UserValidator.PasswordNoLeadingTrailingSpacesMessage, new[] { nameof(Password) });

            if (Regex.IsMatch(username, UserValidator.UsernameNoConsecutivePunctRegex)) yield return new ValidationResult(UserValidator.UsernameNoConsecutivePunctMessage, new[] { nameof(Username) });
            if (Regex.IsMatch(username, UserValidator.UsernameCannotBeEmailRegex)) yield return new ValidationResult(UserValidator.UsernameCannotBeEmailMessage, new[] { nameof(Username) });

            var emailLocal = email.Split('@')[0];
            if (!string.IsNullOrWhiteSpace(username) && password.IndexOf(username, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                yield return new ValidationResult(UserValidator.PasswordNotContainUsernameMessage, new[] { nameof(Password) });
            }

            if (!string.IsNullOrWhiteSpace(emailLocal) && password.IndexOf(emailLocal, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                yield return new ValidationResult(UserValidator.PasswordNotContainEmailLocalMessage, new[] { nameof(Password) });
            }

            if (!string.IsNullOrWhiteSpace(password) && (string.Equals(password, username, StringComparison.OrdinalIgnoreCase) || string.Equals(password, email, StringComparison.OrdinalIgnoreCase)))
            {
                yield return new ValidationResult(UserValidator.PasswordNotEqualUsernameOrEmailMessage, new[] { nameof(Password) });
            }

            if (!Enum.IsDefined(typeof(Role), Role)) yield return new ValidationResult(UserValidator.RoleInvalidMessage, new[] { nameof(Role) });
        }
    }
}
