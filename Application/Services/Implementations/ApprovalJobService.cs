using Application.Abstraction;
using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ApprovalJobService : IApprovalJobService
    {
        private readonly IApprovalJobRepository _approvalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApprovalJobService(IApprovalJobRepository approvalRepository, IUnitOfWork unitOfWork)
        {
            _approvalRepository = approvalRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
