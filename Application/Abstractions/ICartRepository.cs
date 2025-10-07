using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface ICartRepository : IEfRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default);
        Task<Cart> CreateForUserAsync(int userId, CancellationToken ct = default);
    }
}
