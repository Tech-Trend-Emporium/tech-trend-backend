using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class AuthValidator
    {
        // Service validation messages
        public const string RefreshTokenRequiredErrorMessage = "Refresh token is required.";
        public const string InvalidOrExpiredRefreshTokenErrorMessage = "Invalid or expired refresh token.";
        public const string InvalidCredentialsErrorMessage = "The email, username, or password is incorrect.";
        public const string InactiveUserErrorMessage = "The user is not active.";
        public const string RefreshTokenRequiredWhenAllSessionsFalseErrorMessage = "Refresh token required when AllSessions=false.";
        public const string RefreshTokenNotFoundErrorMessage = "Refresh token not found.";
        public const string EmailOrUsernameAlreadyTakenErrorMessage = "The email or username is already taken.";

        // DTO validation messages
        public const int RefreshTokenMinLength = 20;
        public const int RefreshTokenMaxLength = 500;
        public const string RefreshTokenRequiredMessage = "Refresh token is required.";
        public const string RefreshTokenLengthMessage = "Refresh token must be between 20 and 500 characters.";
        public const int EmailOrUsernameMinLength = 3;
        public const int EmailOrUsernameMaxLength = 150;
        public const string EmailOrUsernameRegex = @"^[a-zA-Z0-9@\.\-_]+$";
        public const string EmailOrUsernameRequiredMessage = "Email or username is required.";
        public const string EmailOrUsernameLengthMessage = "Email or username must be between 3 and 150 characters.";
        public const string EmailOrUsernameRegexMessage = "Email or username contains invalid characters.";
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$";
        public const string PasswordRequiredMessage = "Password is required.";
        public const string PasswordLengthMessage = "Password must be between 8 and 100 characters.";
        public const string PasswordRegexMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        public const string AllSessionsRequiredMessage = "The session scope must be specified.";
        public const string RefreshTokenRequiredForSingleLogoutMessage = "The refresh token is required when logging out from the current session only.";
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 50;
        public const string UsernameRegex = @"^[A-Za-z0-9](?:[A-Za-z0-9._-]*[A-Za-z0-9])?$";
        public const string UsernameNoConsecutivePunctRegex = @"[._-]{2,}";
        public const string UsernameCannotBeEmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string UsernameRequiredMessage = "Username is required.";
        public const string UsernameLengthMessage = "Username must be between 3 and 50 characters.";
        public const string UsernameRegexMessage = "Username must start and end with a letter or number and may include '.', '_' or '-' in the middle.";
        public const string UsernameNoLeadingTrailingSpacesMessage = "Username cannot start or end with spaces.";
        public const string UsernameNoConsecutivePunctMessage = "Username cannot contain consecutive '.', '_' or '-'.";
        public const string UsernameCannotBeEmailMessage = "Username cannot be an email address.";
        public const int EmailMaxLength = 254;
        public const string EmailRequiredMessage = "Email is required.";
        public const string EmailInvalidMessage = "Email is not a valid email address.";
        public const string EmailMaxLengthMessage = "Email must be a maximum length of 254 characters.";
        public const string EmailNoLeadingTrailingSpacesMessage = "Email cannot start or end with spaces.";
        public const string PasswordNoLeadingTrailingSpacesMessage = "Password cannot start or end with spaces.";
        public const string PasswordNotContainUsernameMessage = "Password must not contain the username.";
        public const string PasswordNotContainEmailLocalMessage = "Password must not contain part of the email.";
        public const string PasswordNotEqualUsernameOrEmailMessage = "Password cannot be equal to the username or the email.";
    }
}
