using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace General.Dto.Review
{
    public class CreateReviewRequest : IValidatableObject
    {
        [Required(ErrorMessage = UserValidator.UsernameRequiredMessage)]
        [StringLength(UserValidator.UsernameMaxLength, MinimumLength = UserValidator.UsernameMinLength, ErrorMessage = UserValidator.UsernameLengthMessage)]
        [RegularExpression(UserValidator.UsernameRegex, ErrorMessage = UserValidator.UsernameRegexMessage)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = ReviewValidator.ProductIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = ReviewValidator.ProductIdRangeMessage)]
        public int ProductId { get; set; }

        [StringLength(2000, ErrorMessage = ReviewValidator.CommentMaxLengthMessage)]
        public string? Comment { get; set; }

        [Required(ErrorMessage = ReviewValidator.RatingRequiredMessage)]    
        [Range(0, 5, ErrorMessage = ReviewValidator.RatingRangeMessage)]
        public float Rating { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Username != null && Username != Username.Trim()) yield return new ValidationResult(ReviewValidator.UsernameWhitespaceMessage, new[] { nameof(Username) });

            if (Comment != null)
            {
                var trimmed = Comment.Trim();

                if (trimmed.Length == 0) yield return new ValidationResult(ReviewValidator.CommentWhitespaceMessage, new[] { nameof(Comment) });
            }

            if (float.IsNaN(Rating) || float.IsInfinity(Rating)) yield return new ValidationResult(ReviewValidator.RatingInvalidNumberMessage, new[] { nameof(Rating) });
        }
    }
}
