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
    public class WishListRepository : EfRepository<WishList>, IWishListRepository
    {
        private readonly AppDbContext _db;

        public WishListRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<WishList?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default)
        {
            IQueryable<WishList> q = _db.WishLists.Where(w => w.UserId == userId);
            if (includeGraph) q = q.Include(w => w.Items).ThenInclude(i => i.Product);

            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<WishList> CreateForUserAsync(int userId, CancellationToken ct = default)
        {
            var wl = new WishList { UserId = userId, CreatedAt = DateTime.UtcNow };
            _db.WishLists.Add(wl);

            await _db.SaveChangesAsync(ct);

            return wl;
        }
    }
}
