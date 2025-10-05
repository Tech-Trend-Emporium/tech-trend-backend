using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class CategoryRepository : EfRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _db;

        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task<Category?> GetAsync(Expression<Func<Category, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking ? _db.Categories.AsTracking() : _db.Categories.AsNoTracking()).FirstOrDefaultAsync(predicate, ct);
        }

        public Task<IReadOnlyList<Category>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            return Task.FromResult((IReadOnlyList<Category>)_db.Categories.Where(c => ids.Contains(c.Id)).ToList());
        }
    }
}
