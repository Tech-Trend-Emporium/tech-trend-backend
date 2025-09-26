using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Repository.Implementations
{
    public class CouponRepository : ICouponRepository
    {
        public Task<Coupon> AddAsync(Coupon entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Coupon>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Coupon?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Coupon> UpdateAsync(Coupon entity)
        {
            throw new NotImplementedException();
        }
    }
}
