namespace Domain.Validations
{
    public class RecoveryQuestionValidator
    {
        // Service validation messages
        public static string RecoveryQuestionNotFound(int id) => $"The recovery question with id '{id}' not found.";
        public const string RecoveryAnswerIncorrectErrorMessage = "The recovery answer is incorrect.";

        // DTO validation messages
        public const int RecoveryAnswerMinLength = 2;
        public const int RecoveryAnswerMaxLength = 200;
        public const string RecoveryAnswerRequiredMessage = "The field RecoveryAnswer is required.";
        public const string RecoveryAnswerLengthMessage = "The field RecoveryAnswer must be between 2 and 200 characters.";
        public const string QuestionIdRequiredMessage = "A recovery question must be selected.";
        public const string QuestionRequiredMessage = "Question is required.";
        public const string QuestionLengthMessage = "Question must be between 1 and 2000 characters.";
        public const string QuestionWhitespaceMessage = "Question must not have leading or trailing spaces.";
        public const string QuestionAlreadyExistsMessage = "A recovery question with the same text already exists.";
    }
}
