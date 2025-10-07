using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Review
{
    public class UpdateReviewRequest : IValidatableObject
    {
        [StringLength(2000, ErrorMessage = ReviewValidator.CommentMaxLengthMessage)]
        public string? Comment { get; set; }

        [Range(0, 5, ErrorMessage = ReviewValidator.RatingRangeMessage)]
        public float? Rating { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Comment is null && Rating is null) yield return new ValidationResult(ReviewValidator.AtLeastOneFieldMessage, new[] { nameof(Comment), nameof(Rating) });

            if (Comment != null)
            {
                var trimmed = Comment.Trim();

                if (trimmed.Length == 0) yield return new ValidationResult(ReviewValidator.CommentWhitespaceMessage, new[] { nameof(Comment) });
            }

            if (Rating.HasValue && (float.IsNaN(Rating.Value) || float.IsInfinity(Rating.Value))) yield return new ValidationResult(ReviewValidator.RatingInvalidNumberMessage, new[] { nameof(Rating) });
        }
    }
}
