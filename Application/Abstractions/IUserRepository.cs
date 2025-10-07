using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface IUserRepository : IEfRepository<User>
    {
        Task<User?> GetAsync(Expression<Func<User, bool>> predicate, bool asTracking = false, CancellationToken ct = default);
        Task<IReadOnlyList<User>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
    }
}
