using Application.Dtos.ApprovalJob;
using Application.Services;
using Asp.Versioning;
using General.Dto.ApprovalJob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing approval jobs, including submission, retrieval, and decision-making processes.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApprovalJobController : ControllerBase
    {
        private readonly IApprovalJobService _approvalJobService;

        /// <summary>
        /// Retrieves the ID of the currently authenticated user from the JWT claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalJobController"/> class.
        /// </summary>
        /// <param name="approvalJobService">Service that provides business logic for approval job operations.</param>
        public ApprovalJobController(IApprovalJobService approvalJobService)
        {
            _approvalJobService = approvalJobService;
        }

        /// <summary>
        /// Submits a new approval job request on behalf of the current user.
        /// </summary>
        /// <param name="dto">The request data containing the type, operation, and payload for the approval job.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A newly created <see cref="ApprovalJobResponse"/> representing the submitted job.
        /// </returns>
        /// <response code="200">Returns the created approval job.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        [HttpPost]
        [Authorize(Roles = "EMPLOYEE")]
        [ProducesResponseType(typeof(ApprovalJobResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovalJobResponse>> Submit([FromBody] SubmitApprovalJobRequest dto, CancellationToken ct)
        {
            return Ok(await _approvalJobService.SubmitAsync(CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Retrieves a paginated list of all pending approval jobs awaiting a decision.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A read-only list of <see cref="ApprovalJobResponse"/> objects representing pending jobs.</returns>
        /// <response code="200">Returns the list of pending approval jobs.</response>
        [HttpGet("pending")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(IReadOnlyList<ApprovalJobResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ApprovalJobResponse>>> Pending(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            return Ok(await _approvalJobService.ListPendingAsync(skip, take, ct));
        }

        /// <summary>
        /// Allows an administrator to approve or reject a specific approval job.
        /// </summary>
        /// <param name="id">The unique identifier of the approval job to decide on.</param>
        /// <param name="dto">The decision data indicating approval or rejection and an optional reason.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="ApprovalJobResponse"/> reflecting the decision result.</returns>
        /// <response code="200">Returns the updated approval job with the decision applied.</response>
        /// <response code="404">If the approval job does not exist.</response>
        /// <response code="409">If the approval job has already been decided.</response>
        [HttpPost("{id:int}/decision")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApprovalJobResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApprovalJobResponse>> Decide(
            [FromRoute] int id,
            [FromBody] DecideApprovalJobRequest dto,
            CancellationToken ct)
        {
            return Ok(await _approvalJobService.DecideAsync(id, CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Retrieves a specific approval job by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the approval job to retrieve.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="ApprovalJobResponse"/> if found; otherwise, <c>404 Not Found</c>.
        /// </returns>
        /// <response code="200">Returns the approval job if found.</response>
        /// <response code="404">If the approval job is not found.</response>
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
