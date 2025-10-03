using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class SignInRequest
    {
        [Required(ErrorMessage = AuthValidator.EmailOrUsernameRequiredMessage)]
        [StringLength(AuthValidator.EmailOrUsernameMaxLength, MinimumLength = AuthValidator.EmailOrUsernameMinLength, ErrorMessage = AuthValidator.EmailOrUsernameLengthMessage)]
        [RegularExpression(AuthValidator.EmailOrUsernameRegex, ErrorMessage = AuthValidator.EmailOrUsernameRegexMessage)]
        public string EmailOrUsername { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.PasswordRequiredMessage)]
        [StringLength(AuthValidator.PasswordMaxLength, MinimumLength = AuthValidator.PasswordMinLength, ErrorMessage = AuthValidator.PasswordLengthMessage)]
        [RegularExpression(AuthValidator.PasswordRegex, ErrorMessage = AuthValidator.PasswordRegexMessage)]
        public string Password { get; set; } = null!;
    }
}
