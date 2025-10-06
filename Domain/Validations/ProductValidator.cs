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
        public const string CountNegativeErrorMessage = "Count cannot be negative.";

        // Service validation messages
        public static string ProductNotFound(int id) => $"The product with id '{id}' not found.";
        public static string ProductAlreadyExists(string title) => $"The product with title '{title}' already exists.";

        // DTO validation messages
        public const string TitleRequiredMessage = "Title is required.";
        public const string TitleMaxLengthMessage = "Title must be a maximum length of 200 characters.";
        public const string TitleNoLeadingTrailingSpacesMessage = "Title cannot contain leading or trailing spaces.";
        public const string PriceRequiredMessage = "Price is required.";
        public const string PriceRangeMessage = "Price must be between 0 and 999999999999.99.";
        public const string DescriptionMaxLengthMessage = "Description must be a maximum length of 2000 characters.";
        public const string DescriptionNoLeadingTrailingSpacesMessage = "Description cannot contain leading or trailing spaces.";
        public const string ImageUrlInvalidMessage = "ImageUrl must be a valid URL.";
        public const string ImageUrlMaxLengthMessage = "ImageUrl must be a maximum length of 2048 characters.";
        public const string ImageUrlNoLeadingTrailingSpacesMessage = "ImageUrl cannot contain leading or trailing spaces.";
        public const string RatingRangeMessage = "RatingRate must be between 0 and 5.";
        public const string CountNonNegativeMessage = "Count cannot be negative.";
        public const int CategoryMinLength = 3;
        public const int CategoryMaxLength = 120;
        public const string CategoryRegex = @"^[a-zA-Z0-9\s\-\.\,&]+$";
        public const string CategoryRequiredMessage = "Category is required.";
        public const string CategoryLengthMessage = "Category must be between 3 and 120 characters long.";
        public const string CategoryRegexMessage = "Category can only contain letters, numbers, spaces, and the following characters: - . , &";
        public const string CategoryNoLeadingTrailingSpacesMessage = "Category cannot contain leading or trailing spaces.";
    }
}
