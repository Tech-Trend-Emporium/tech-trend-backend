using Data.Entities;
using General.Dto.Product;
using General.Dto.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateAsync(CreateReviewRequest dto,CancellationToken ct = default);

        Task<IReadOnlyList<Review>> ListAsync(int skip = 0, int take = 50, int productId = 0, CancellationToken ct = default);
    }
}
