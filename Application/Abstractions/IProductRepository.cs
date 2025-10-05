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
    }
}
