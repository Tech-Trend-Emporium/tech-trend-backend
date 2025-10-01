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
        [Required(ErrorMessage = "The email or username is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "The email or username must be between 3 and 150 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9@\.\-_]+$", ErrorMessage = "The email or username contains invalid characters.")]
        public string EmailOrUsername { get; set; } = null!;

        [Required(ErrorMessage = "The password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;
    }
}
