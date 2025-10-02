using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "The refresh token is required.")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "The refresh token must be between 20 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\-_\.]+$", ErrorMessage = "The refresh token contains invalid characters.")]
        public string RefreshToken { get; set; } = null!;
    }
}
