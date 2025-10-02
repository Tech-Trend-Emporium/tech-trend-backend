using Application.Abstraction;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;

        public EfRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate != null) return _dbSet.CountAsync(predicate, ct);

            return _dbSet.CountAsync(ct);
        }

        public async Task<bool> DeleteByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            var entity = await _dbSet.FindAsync(keyValues, ct).AsTask();
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().AnyAsync(predicate, ct);
        }

        public Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues)
        {
            return _dbSet.FindAsync(keyValues, ct).AsTask();
        }

        public Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().Skip(skip).Take(take).ToListAsync(ct).ContinueWith<IReadOnlyList<T>>(t => (IReadOnlyList<T>)t.Result, ct);
        }

        public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().Where(predicate).Skip(skip).Take(take).ToListAsync(ct).ContinueWith<IReadOnlyList<T>>(t => (IReadOnlyList<T>)t.Result, ct);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
