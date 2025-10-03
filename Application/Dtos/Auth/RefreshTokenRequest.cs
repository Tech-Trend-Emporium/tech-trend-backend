using Domain.Validations;
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
        [Required(ErrorMessage = AuthValidator.RefreshTokenRequiredMessage)]
        [StringLength(AuthValidator.RefreshTokenMaxLength, MinimumLength = AuthValidator.RefreshTokenMinLength, ErrorMessage = AuthValidator.RefreshTokenLengthMessage)]
        public string RefreshToken { get; set; } = null!;
    }
}
