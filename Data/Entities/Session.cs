using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Session : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public DateTime LoginAt { get; set; } = DateTime.UtcNow;

        public DateTime? LogoutAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (LogoutAt.HasValue && LogoutAt < LoginAt)
                yield return new ValidationResult("LogoutAt no puede ser anterior a LoginAt", new[] { nameof(LogoutAt), nameof(LoginAt) });
        }
    }
}
