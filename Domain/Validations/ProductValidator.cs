using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class ProductValidator
    {
        // Entity validation messages
        public const string CountNegativeErrorMessage = "The field Count cannot be negative.";

        // Service validation messages
        public static string ProductNotFound(int id) => $"The product with id '{id}' not found.";
        public static string ProductAlreadyExists(string title) => $"The product with title '{title}' already exists.";

        // DTO validation messages
        public const string TitleRequiredMessage = "The field Title is required.";
        public const string TitleMaxLengthMessage = "The field Title must be a maximum length of 200 characters.";
        public const string TitleNoLeadingTrailingSpacesMessage = "The field Title cannot contain leading or trailing spaces.";
        public const string PriceRequiredMessage = "The field Price is required.";
        public const string PriceRangeMessage = "The field Price must be between 0 and 999999999999.99.";
        public const string DescriptionMaxLengthMessage = "The field Description must be a maximum length of 2000 characters.";
        public const string DescriptionNoLeadingTrailingSpacesMessage = "The field Description cannot contain leading or trailing spaces.";
        public const string ImageUrlInvalidMessage = "The field ImageUrl must be a valid URL.";
        public const string ImageUrlMaxLengthMessage = "The field ImageUrl must be a maximum length of 2048 characters.";
        public const string ImageUrlNoLeadingTrailingSpacesMessage = "The field ImageUrl cannot contain leading or trailing spaces.";
        public const string RatingRangeMessage = "The field RatingRate must be between 0 and 5.";
        public const string CountNonNegativeMessage = "The field Count cannot be negative.";
        public const int CategoryMinLength = 3;
        public const int CategoryMaxLength = 120;
        public const string CategoryRegex = @"^[a-zA-Z0-9\s\-\.\,&]+$";
        public const string CategoryRequiredMessage = "The field Category is required.";
        public const string CategoryLengthMessage = "The field Category must be between 3 and 120 characters long.";
        public const string CategoryRegexMessage = "The field Category can only contain letters, numbers, spaces, and the following characters: - . , &";
        public const string CategoryNoLeadingTrailingSpacesMessage = "The field Category cannot contain leading or trailing spaces.";
    }
}
