namespace Domain.Validations
{
    public class CategoryValidator
    {
        // Service validation messages
        public static string CategoryNameExists(string name) => $"The category with name '{name}' already exists.";
        public static string CategoryNotFound(int id) => $"The category with id '{id}' not found.";
        public static string CategoryNotFound(string name) => $"The category with name '{name}' not found.";

        // DTO validation messages
        public const int NameMinLength = 3;
        public const int NameMaxLength = 120;
        public const string NameRegex = @"^[a-zA-Z0-9\s\-\.\,&']+$";
        public const string NameRequiredMessage = "The field Name is required.";
        public const string NameLengthMessage = "The field Name must be between 3 and 120 characters long.";
        public const string NameRegexMessage = "The field Name can only contain letters, numbers, spaces, and the following characters: - . , &";
    }
}
