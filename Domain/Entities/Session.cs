using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Session : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
                yield return new ValidationResult("The field LogoutAt cannot be before LoginAt field.", new[] { nameof(LogoutAt), nameof(LoginAt) });
        }
    }
}
