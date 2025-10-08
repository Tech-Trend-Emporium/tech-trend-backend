using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction
{
    public interface IApprovalJobRepository : IEfRepository<ApprovalJob>
    {
        Task<ApprovalJob?> GetByIdTrackedAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<ApprovalJob>> ListPendingAsync(int skip, int take, CancellationToken ct = default);
    }
}
