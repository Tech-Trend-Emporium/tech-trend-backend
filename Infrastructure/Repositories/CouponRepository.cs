using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class CouponRepository : EfRepository<Coupon>, ICouponRepository
    {
        private readonly AppDbContext _db;

        public CouponRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task<Coupon?> GetAsync(Expression<Func<Coupon, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
