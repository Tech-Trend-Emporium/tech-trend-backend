using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.RecoveryQuestion
{
    public class CreateRecoveryQuestionRequest : IValidatableObject
    {
        [Required(ErrorMessage = RecoveryQuestionValidator.QuestionRequiredMessage)]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = RecoveryQuestionValidator.QuestionLengthMessage)]
        public string Question { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Question != null && Question != Question.Trim())
                yield return new ValidationResult(RecoveryQuestionValidator.QuestionWhitespaceMessage, new[] { nameof(Question) });
        }
    }
}
