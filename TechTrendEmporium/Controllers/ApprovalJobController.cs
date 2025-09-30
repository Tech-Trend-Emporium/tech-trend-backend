using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalJobController
    {
        private readonly IApprovalJobService _approvalJobService;

        public ApprovalJobController(IApprovalJobService approvalJobService)
        {
            _approvalJobService = approvalJobService;
        }
    }
}
