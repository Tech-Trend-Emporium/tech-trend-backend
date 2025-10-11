using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class SignInRequest
    {
        [Required(ErrorMessage = AuthValidator.EmailOrUsernameRequiredMessage)]
        [StringLength(AuthValidator.EmailOrUsernameMaxLength, MinimumLength = AuthValidator.EmailOrUsernameMinLength, ErrorMessage = AuthValidator.EmailOrUsernameLengthMessage)]
        [RegularExpression(AuthValidator.EmailOrUsernameRegex, ErrorMessage = AuthValidator.EmailOrUsernameRegexMessage)]
        public string EmailOrUsername { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.PasswordRequiredMessage)]
        public string Password { get; set; } = null!;
    }
}
