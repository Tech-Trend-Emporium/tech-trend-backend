using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface ICouponRepository : IEfRepository<Coupon>
    {
        Task<Coupon?> GetAsync(Expression<Func<Coupon, bool>> predicate, bool asTracking = false, CancellationToken ct = default);
    }
}
