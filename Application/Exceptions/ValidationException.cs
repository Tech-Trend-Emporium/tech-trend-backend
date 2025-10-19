namespace Application.Exceptions
{
    public class ValidationException : DomainException
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException(IDictionary<string, string[]> errors, string message = "Validation failed")
            : base(message) => Errors = errors;
    }
}
