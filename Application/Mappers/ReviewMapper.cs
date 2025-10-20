using Data.Entities;
using General.Dto.Review;

namespace General.Mappers
{
    public static class ReviewMapper
    {
        public static Review ToEntity(CreateReviewRequest dto, int userId)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            return new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment,
                Rating = dto.Rating
            };
        }

        public static void ApplyUpdate(Review entity, UpdateReviewRequest dto)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            if (dto.Comment != null)
            {
                var trimmed = dto.Comment.Trim();

                entity.Comment = trimmed.Length == 0 ? null : trimmed;
            }

            if (dto.Rating.HasValue) entity.Rating = dto.Rating.Value;
        }

        public static ReviewResponse ToResponse(Review entity, string? username = null)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            return new ReviewResponse
            {
                Id = entity.Id,
                Username = username ?? entity.User?.Username ?? "Unknown",
                ProductId = entity.ProductId,
                Comment = entity.Comment ?? string.Empty,
                Rating = entity.Rating
            };
        }

        public static List<ReviewResponse> ToResponseList(IReadOnlyList<Review> entities, IReadOnlyDictionary<int, string> usernamesByUserId)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));
            if (usernamesByUserId is null) throw new ArgumentNullException(nameof(usernamesByUserId));

            var list = new List<ReviewResponse>(entities.Count);
            foreach (var e in entities)
            {
                usernamesByUserId.TryGetValue(e.UserId, out var name);
                list.Add(ToResponse(e, name));
            }

            return list;
        }
    }
}
