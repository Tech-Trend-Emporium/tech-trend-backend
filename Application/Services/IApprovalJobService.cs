using Application.Dtos.ApprovalJob;
using General.Dto.ApprovalJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IApprovalJobService
    {
        Task<ApprovalJobResponse> SubmitAsync(int requesterUserId, SubmitApprovalJobRequest dto, CancellationToken ct = default);
        Task<ApprovalJobResponse> DecideAsync(int jobId, int adminUserId, DecideApprovalJobRequest dto, CancellationToken ct = default);
        Task<IReadOnlyList<ApprovalJobResponse>> ListPendingAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<ApprovalJobResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
