using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface ISessionRepository : IEfRepository<Session>
    {
        Task<List<Session>> GetActiveByUserAsync(int userId, CancellationToken ct = default);
    }
}
