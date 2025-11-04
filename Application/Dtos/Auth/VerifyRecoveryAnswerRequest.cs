using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth
{
    public class VerifyRecoveryAnswerRequest : IValidatableObject
    {
        [Required(ErrorMessage = AuthValidator.EmailOrUsernameRequiredMessage)]
        [StringLength(AuthValidator.EmailOrUsernameMaxLength, MinimumLength = AuthValidator.EmailOrUsernameMinLength, ErrorMessage = AuthValidator.EmailOrUsernameLengthMessage)]
        [RegularExpression(AuthValidator.EmailOrUsernameRegex, ErrorMessage = AuthValidator.EmailOrUsernameRegexMessage)]
        public string EmailOrUsername { get; set; } = null!;

        [Required(ErrorMessage = AuthValidator.QuestionIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = AuthValidator.QuestionIdRangeMessage)]
        public int RecoveryQuestionId { get; set; }

        [Required(ErrorMessage = AuthValidator.AnswerRequiredMessage)]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = AuthValidator.AnswerLengthMessage)]
        public string Answer { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (EmailOrUsername != null && EmailOrUsername != EmailOrUsername.Trim()) yield return new ValidationResult(AuthValidator.EmailOrUsernameWhitespaceMessage, new[] { nameof(EmailOrUsername) });

            if (Answer != null && Answer != Answer.Trim()) yield return new ValidationResult(AuthValidator.AnswerWhitespaceMessage, new[] { nameof(Answer) });
            if (RecoveryQuestionId <= 0) yield return new ValidationResult(AuthValidator.QuestionIdPositiveMessage, new[] { nameof(RecoveryQuestionId) });
        }
    }
}
