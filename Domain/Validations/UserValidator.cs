using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public class UserValidator
    {
        // Service validation messages
        public static string UserUsernameExists(string username) => $"The user with username '{username}' already exists.";
        public static string UserEmailExists(string email) => $"The user with email '{email}' already exists.";
        public static string UserNotFound(int id) => $"The user with id '{id}' not found.";

        // DTO validation messages
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 50;
        public const string UsernameRegex = @"^[A-Za-z0-9](?:[A-Za-z0-9._-]*[A-Za-z0-9])?$";
        public const string UsernameNoConsecutivePunctRegex = @"[._-]{2,}";
        public const string UsernameCannotBeEmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string UsernameRequiredMessage = "The username is required.";
        public const string UsernameLengthMessage = "The username must be between 3 and 50 characters.";
        public const string UsernameRegexMessage = "The username must start and end with a letter or number and may include '.', '_' or '-' in the middle.";
        public const string UsernameNoLeadingTrailingSpacesMessage = "The username cannot start or end with spaces.";
        public const string UsernameNoConsecutivePunctMessage = "The username cannot contain consecutive '.', '_' or '-'.";
        public const string UsernameCannotBeEmailMessage = "The username cannot be an email address.";
        public const int EmailMaxLength = 254;
        public const string EmailRequiredMessage = "The email is required.";
        public const string EmailInvalidMessage = "The email is not a valid email address.";
        public const string EmailMaxLengthMessage = "The email must be a maximum length of 254 characters.";
        public const string EmailNoLeadingTrailingSpacesMessage = "The email cannot start or end with spaces.";
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$";
        public const string PasswordRequiredMessage = "The password is required.";
        public const string PasswordLengthMessage = "The password must be between 8 and 100 characters.";
        public const string PasswordRegexMessage = "The password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        public const string PasswordNoLeadingTrailingSpacesMessage = "The password cannot start or end with spaces.";
        public const string PasswordNotContainUsernameMessage = "The password must not contain the username.";
        public const string PasswordNotContainEmailLocalMessage = "The password must not contain part of the email.";
        public const string PasswordNotEqualUsernameOrEmailMessage = "The password cannot be equal to the username or the email.";
        public const string RoleInvalidMessage = "The provided role is not valid.";
    }
}
