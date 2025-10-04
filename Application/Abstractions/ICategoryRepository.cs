using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface ICategoryRepository : IEfRepository<Category>
    {
        Task<IReadOnlyList<Category>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
    }
}
