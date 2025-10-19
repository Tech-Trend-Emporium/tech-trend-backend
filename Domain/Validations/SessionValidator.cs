namespace Domain.Validations
{
    public static class SessionValidator
    {
        // Entity validation messages
        public const string LogoutAtBeforeLoginAtErrorMessage = "The field LogoutAt cannot be before LoginAt field.";
    }
}
