using General.Dto.Coupon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICouponService
    {
        Task<CouponResponse> CreateAsync(CreateCouponRequest dto, CancellationToken ct = default);
        Task<CouponResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<CouponResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<CouponResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<CouponResponse> UpdateAsync(int id, UpdateCouponRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
