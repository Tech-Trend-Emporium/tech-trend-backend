using Application.Dtos.ApprovalJob;
using Application.Services;
using Application.Services.Implementations;
using Asp.Versioning;
using General.Dto.ApprovalJob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApprovalJobController : ControllerBase
    {
        private readonly IApprovalJobService _approvalJobService;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public ApprovalJobController(IApprovalJobService approvalJobService)
        {
            _approvalJobService = approvalJobService;
        }

        [HttpPost]
        [Authorize(Roles = "EMPLOYEE")]
        [ProducesResponseType(typeof(ApprovalJobResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovalJobResponse>> Submit([FromBody] SubmitApprovalJobRequest dto, CancellationToken ct)
        {
            return Ok(await _approvalJobService.SubmitAsync(CurrentUserId, dto, ct));
        }

        [HttpGet("pending")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(IReadOnlyList<ApprovalJobResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ApprovalJobResponse>>> Pending([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            return Ok(await _approvalJobService.ListPendingAsync(skip, take, ct));
        }

        [HttpPost("{id:int}/decision")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApprovalJobResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovalJobResponse>> Decide([FromRoute] int id, [FromBody] DecideApprovalJobRequest dto, CancellationToken ct)
        {
            return Ok(await _approvalJobService.DecideAsync(id, CurrentUserId, dto, ct));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [ProducesResponseType(typeof(ApprovalJobResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovalJobResponse?>> Get([FromRoute] int id, CancellationToken ct)
        {
            var result = await _approvalJobService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }
    }
}
