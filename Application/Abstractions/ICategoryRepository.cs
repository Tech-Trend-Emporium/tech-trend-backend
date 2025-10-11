using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface ICategoryRepository : IEfRepository<Category>
    {
        Task<Category?> GetAsync(Expression<Func<Category, bool>> predicate, bool asTracking = false, CancellationToken ct = default);
        Task<IReadOnlyList<Category>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
    }
}
