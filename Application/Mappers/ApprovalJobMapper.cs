using Data.Entities;
using General.Dto.ApprovalJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Mappers
{
    public static class ApprovalJobMapper
    {
        public static ApprovalJobResponse ToResponse(ApprovalJob entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            return new ApprovalJobResponse
            {
                Id = entity.Id,
                Type = entity.Type.ToString(),
                Operation = entity.Operation.ToString(),
                State = entity.State,
                RequestedBy = entity.RequestedBy,
                DecidedBy = entity.DecidedBy,
                RequestedAt = entity.RequestedAt,
                DecidedAt = entity.DecidedAt,
                TargetId = entity.TargetId,
                Reason = entity.Reason
            };
        }

        public static List<ApprovalJobResponse> ToResponseList(IEnumerable<ApprovalJob> entities)
        {
            return entities.Select(ToResponse).ToList();
        }
    }
}
