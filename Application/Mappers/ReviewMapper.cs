using Data.Entities;
using General.Dto.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class ReviewMapper
    {
        // Map CreateReviewRequest to Review entity
        public static Review ToEntity(CreateReviewRequest dto, int id)
        {
            return new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                ProductId = dto.ProductId,
                UserId = id
            };
        }
        // Map Review entity to ReviewResponse
        public static ReviewResponse ToResponse(Review entity, string username)
        {
            return new ReviewResponse
            {
                Id = entity.Id,
                Rating = entity.Rating,
                Comment = entity.Comment,
                ProductId = entity.ProductId,
                Username = username
            };
        }
    }
}
