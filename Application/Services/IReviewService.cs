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
        Task<ReviewResponse> CreateAsync(CreateReviewRequest dto, CancellationToken ct = default);
        Task<ReviewResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<ReviewResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<IReadOnlyList<ReviewResponse>> ListByProductAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<ReviewResponse> Items, int Total)> ListByProductWithCountAsync(int productId, int skip = 0, int take = 50, CancellationToken ct = default);
        Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
