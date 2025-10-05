using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface IProductRepository : IEfRepository<Product>
    {
        Task<IReadOnlyList<Product>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null);
        Task<IReadOnlyList<Product>> ListAsync(int skip = 0, int take = 50, int? categoryId = null, CancellationToken ct = default);
        Task<int> CountAsync(int? categoryId = null, CancellationToken ct = default);
    }
}
