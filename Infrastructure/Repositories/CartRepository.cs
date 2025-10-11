using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class CartRepository : EfRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _db;

        public CartRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Cart> CreateForUserAsync(int userId, CancellationToken ct = default)
        {
            var cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
            
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);
            
            return cart;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default)
        {
            IQueryable<Cart> q = _db.Carts.Where(c => c.UserId == userId);
            
            if (includeGraph) q = q.Include(c => c.Items).ThenInclude(i => i.Product).Include(c => c.Coupon);
            
            return await q.FirstOrDefaultAsync(ct);
        }
    }
}
