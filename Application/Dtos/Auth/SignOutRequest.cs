using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class SignOutRequest : IValidatableObject
    {
        [StringLength(500, MinimumLength = 20, ErrorMessage = "The refresh token must be between 20 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\-_\.]+$", ErrorMessage = "The refresh token contains invalid characters.")]
        public string? RefreshToken { get; set; }

        [Required(ErrorMessage = "The session scope must be specified.")]
        public bool AllSessions { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AllSessions && string.IsNullOrWhiteSpace(RefreshToken))
            {
                yield return new ValidationResult(
                    "The refresh token is required when logging out from the current session only.",
                    new[] { nameof(RefreshToken) });
            }
        }
    }
}
