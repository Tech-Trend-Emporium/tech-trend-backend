using Domain.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
            if (LogoutAt.HasValue && LogoutAt < LoginAt) yield return new ValidationResult(SessionValidator.LogoutAtBeforeLoginAtErrorMessage, new[] { nameof(LogoutAt), nameof(LoginAt) });
        }
    }
}
