using Domain.Enums;
using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace General.Dto.User
{
    public class UpdateUserRequest
    {
        [StringLength(UserValidator.UsernameMaxLength, MinimumLength = UserValidator.UsernameMinLength, ErrorMessage = UserValidator.UsernameLengthMessage)]
        [RegularExpression(UserValidator.UsernameRegex, ErrorMessage = UserValidator.UsernameRegexMessage)]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = UserValidator.EmailInvalidMessage)]
        [StringLength(UserValidator.EmailMaxLength, ErrorMessage = UserValidator.EmailMaxLengthMessage)]
        public string? Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}
