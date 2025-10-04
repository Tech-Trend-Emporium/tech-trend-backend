using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface IEfRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues);
        Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int skip = 0, int take = 50, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        Task<bool> DeleteByIdAsync(CancellationToken ct = default, params object[] keyValues);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
