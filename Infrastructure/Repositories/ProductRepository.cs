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
using System.Linq.Expressions;

namespace Application.Repository
{
    public class ProductRepository : EfRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task<Product?> GetAsync(Expression<Func<Product, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking ? _db.Products.AsTracking() : _db.Products.AsNoTracking()).FirstOrDefaultAsync(predicate, ct);
        }

        public Task<IReadOnlyList<Product>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            return Task.FromResult((IReadOnlyList<Product>)_db.Products.Where(c => ids.Contains(c.Id)).ToList());
        }

        public Task<IReadOnlyList<Product>> ListAsync(int skip = 0, int take = 50, int? categoryId = null, CancellationToken ct = default)
        {
            if (categoryId.HasValue) return base.ListAsync(p => p.CategoryId == categoryId.Value, skip, take, ct);

            return base.ListAsync(skip, take, ct);
        }

        public Task<int> CountAsync(int? categoryId = null, CancellationToken ct = default)
        {
            if (categoryId.HasValue) return base.CountAsync(p => p.CategoryId == categoryId.Value, ct);

            return base.CountAsync(null, ct);
        }
    }
}
