using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApprovalJobController
    {
        private readonly IApprovalJobService _approvalJobService;

        public ApprovalJobController(IApprovalJobService approvalJobService)
        {
            _approvalJobService = approvalJobService;
        }
    }
}
