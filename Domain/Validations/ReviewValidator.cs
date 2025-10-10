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
        public const string ProductIdRequiredMessage = "The field ProductId is required.";
        public const string ProductIdRangeMessage = "The field ProductId must be a positive integer.";
        public const string CommentMaxLengthMessage = "The field Comment must be a maximum length of 2000 characters.";
        public const string RatingRequiredMessage = "The field Rating is required.";
        public const string RatingRangeMessage = "The field Rating must be between 0 and 5.";
        public const string UsernameWhitespaceMessage = "The field Username cannot start or end with spaces.";
        public const string CommentWhitespaceMessage = "The field Comment cannot be only whitespace.";
        public const string RatingInvalidNumberMessage = "The field Rating must be a valid number.";
        public const string AtLeastOneFieldMessage = "At least one field (comment or rating) must be provided.";
    }
}
