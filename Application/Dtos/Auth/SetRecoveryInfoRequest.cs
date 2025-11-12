using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class SetRecoveryInfoRequest : IValidatableObject
    {
        [Required(ErrorMessage = AuthValidator.QuestionIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = AuthValidator.QuestionIdRangeMessage)]
        public int RecoveryQuestionId { get; set; }

        [Required(ErrorMessage = AuthValidator.AnswerRequiredMessage)]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = AuthValidator.AnswerLengthMessage)]
        public string Answer { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Answer != null && Answer != Answer.Trim()) yield return new ValidationResult(AuthValidator.AnswerWhitespaceMessage, new[] { nameof(Answer) });
            if (RecoveryQuestionId <= 0) yield return new ValidationResult(AuthValidator.QuestionIdPositiveMessage, new[] { nameof(RecoveryQuestionId) });
        }
    }
}
