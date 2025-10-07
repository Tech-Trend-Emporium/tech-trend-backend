using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public class ReviewValidator
    {
        // Service validation messages
        public static string ReviewNotFound(int id) => $"The review with ID {id} was not found.";
        public static string ReviewAlreadyExistsForUserAndProduct(string username, int productId) => $"A review by user '{username}' for product ID {productId} already exists.";

        // DTO validation messages
        public const string ProductIdRequiredMessage = "The product ID is required.";
        public const string ProductIdRangeMessage = "The product ID must be a positive integer.";
        public const string CommentMaxLengthMessage = "The comment must be a maximum length of 2000 characters.";
        public const string RatingRequiredMessage = "The rating is required.";
        public const string RatingRangeMessage = "The rating must be between 0 and 5.";
        public const string UsernameWhitespaceMessage = "The username cannot start or end with spaces.";
        public const string CommentWhitespaceMessage = "The comment cannot be only whitespace.";
        public const string RatingInvalidNumberMessage = "The rating must be a valid number.";
        public const string AtLeastOneFieldMessage = "At least one field (comment or rating) must be provided.";
    }
}
