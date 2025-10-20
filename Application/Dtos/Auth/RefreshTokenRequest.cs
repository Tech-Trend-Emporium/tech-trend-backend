using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = AuthValidator.RefreshTokenRequiredMessage)]
        [StringLength(AuthValidator.RefreshTokenMaxLength, MinimumLength = AuthValidator.RefreshTokenMinLength, ErrorMessage = AuthValidator.RefreshTokenLengthMessage)]
        public string RefreshToken { get; set; } = null!;
    }
}
