using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ApprovalJob : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public ApprovalJobType Type { get; set; }

        [Required]
        public Operation Operation { get; set; } 

        [Required]
        public bool State { get; set; } = false; 

        [Required]
        public int RequestedBy { get; set; }
        public User RequestedByUser { get; set; } = null!;

        public int? DecidedBy { get; set; }
        public User? DecidedByUser { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DecidedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (DecidedAt.HasValue && !DecidedBy.HasValue)
                yield return new ValidationResult("DecidedAt requiere DecidedBy.", new[] { nameof(DecidedAt), nameof(DecidedBy) });

            if (DecidedAt.HasValue && DecidedAt < RequestedAt)
                yield return new ValidationResult("DecidedAt no puede ser anterior a RequestedAt.", new[] { nameof(DecidedAt), nameof(RequestedAt) });

            if (DecidedBy.HasValue && DecidedBy == RequestedBy)
                yield return new ValidationResult("DecidedBy no puede ser el mismo que RequestedBy.", new[] { nameof(DecidedBy), nameof(RequestedBy) });
        }
    }
}
