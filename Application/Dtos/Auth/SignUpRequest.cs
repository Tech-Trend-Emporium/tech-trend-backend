using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class SignUpRequest : IValidatableObject
    {
        [Required(ErrorMessage = "The username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9](?:[A-Za-z0-9._-]*[A-Za-z0-9])?$", ErrorMessage = "The username must start and end with a letter or number and may include '.', '_' or '-' in the middle.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email is not a valid email address.")]
        [StringLength(254, ErrorMessage = "The email must be a maximum length of 254 characters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "The password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])(?!.*\s).+$", ErrorMessage = "The password must contain uppercase, lowercase, number, special character, and no spaces.")]
        public string Password { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var username = Username ?? string.Empty;
            var email = Email ?? string.Empty;
            var password = Password ?? string.Empty;

            if (Username != null && (Username != Username.Trim()))
                yield return new ValidationResult("The username cannot start or end with spaces.", new[] { nameof(Username) });

            if (Email != null && (Email != Email.Trim()))
                yield return new ValidationResult("The email cannot start or end with spaces.", new[] { nameof(Email) });

            if (Password != null && (Password != Password.Trim()))
                yield return new ValidationResult("The password cannot start or end with spaces.", new[] { nameof(Password) });

            if (Regex.IsMatch(username, @"[._-]{2,}"))
                yield return new ValidationResult("The username cannot contain consecutive '.', '_' or '-'.", new[] { nameof(Username) });

            if (Regex.IsMatch(username, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                yield return new ValidationResult("The username cannot be an email address.", new[] { nameof(Username) });

            var emailLocal = email.Split('@')[0];
            if (!string.IsNullOrWhiteSpace(username) && password.IndexOf(username, StringComparison.OrdinalIgnoreCase) >= 0)
                yield return new ValidationResult("The password must not contain the username.", new[] { nameof(Password) });

            if (!string.IsNullOrWhiteSpace(emailLocal) && password.IndexOf(emailLocal, StringComparison.OrdinalIgnoreCase) >= 0)
                yield return new ValidationResult("The password must not contain part of the email.", new[] { nameof(Password) });

            if (!string.IsNullOrWhiteSpace(password) &&
                (string.Equals(password, username, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(password, email, StringComparison.OrdinalIgnoreCase)))
            {
                yield return new ValidationResult("The password cannot be equal to the username or the email.", new[] { nameof(Password) });
            }
        }
    }
}
