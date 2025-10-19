using Application.Dtos.ApprovalJob;
using Application.Services;
using Asp.Versioning;
using Domain.Enums;
using Domain.Validations;
using General.Dto.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// Exposes endpoints to manage product categories including retrieval,
    /// listing (with total count), creation, update, and deletion.
    /// When the caller is an EMPLOYEE, create/delete operations are submitted as approval jobs.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IApprovalJobService _approvalJobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">Business service for category operations.</param>
        /// <param name="approvalJobService">Service used to submit approval jobs for non-admin requests.</param>
        public CategoryController(ICategoryService categoryService, IApprovalJobService approvalJobService)
        {
            _categoryService = categoryService;
            _approvalJobService = approvalJobService;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from the JWT claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Indicates whether the current user has the ADMIN role.
        /// </summary>
        private bool IsAdmin => User.IsInRole("ADMIN");

        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns><c>200 OK</c> with the category if found; otherwise <c>404 Not Found</c>.</returns>
        /// <response code="200">Returns the category.</response>
        /// <response code="404">If the category does not exist.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of categories along with the total count.
        /// </summary>
        /// <param name="skip">Number of records to skip. Defaults to 0.</param>
        /// <param name="take">Number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns><c>200 OK</c> with an object containing <c>Total</c> and <c>Items</c>.</returns>
        /// <response code="200">Returns the page of categories with total count.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE, SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _categoryService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Creates a new category. 
        /// Admins execute immediately; employees submit an approval job.
        /// </summary>
        /// <param name="dto">The category creation payload.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description><c>201 Created</c> with the created category (when executed directly by ADMIN).</description></item>
        /// <item><description><c>202 Accepted</c> with approval job details (when submitted by EMPLOYEE).</description></item>
        /// </list>
        /// </returns>
        /// <response code="201">Category created successfully (ADMIN).</response>
        /// <response code="202">Approval requested (EMPLOYEE).</response>
        /// <response code="400">Validation error.</response>
        /// <response code="409">Category name already exists.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest dto, CancellationToken ct)
        {
            return await ExecuteOrRequestApprovalAsync(
                directAction: async token => await _categoryService.CreateAsync(dto, token),
                buildApprovalRequest: () => new SubmitApprovalJobRequest
                {
                    Type = ApprovalJobType.CATEGORY,
                    Operation = Operation.CREATE,
                    Payload = dto,
                    Reason = CommonValidator.CategoryCreationRequestedByEmployee(CurrentUserId)
                },
                approvalMessage: CommonValidator.CategoryCreationValidationMessage,
                ct: ct,
                onSuccess: created => CreatedAtAction(nameof(GetById), new { id = created.Id }, created)
            );
        }

        /// <summary>
        /// Updates an existing category. Only ADMIN and EMPLOYEE can perform this action directly.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="dto">The update payload.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns><c>200 OK</c> with the updated category.</returns>
        /// <response code="200">Category updated successfully.</response>
        /// <response code="404">Category not found.</response>
        /// <response code="409">Name already in use.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest dto, CancellationToken ct)
        {
            var updated = await _categoryService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a category. 
        /// Admins execute immediately; employees submit an approval job.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description><c>204 No Content</c> if deleted (ADMIN).</description></item>
        /// <item><description><c>404 Not Found</c> if the category does not exist (ADMIN path).</description></item>
        /// <item><description><c>202 Accepted</c> with approval job details (EMPLOYEE).</description></item>
        /// </list>
        /// </returns>
        /// <response code="204">Deleted successfully (ADMIN).</response>
        /// <response code="202">Approval requested (EMPLOYEE).</response>
        /// <response code="404">Category not found (ADMIN path).</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            return await ExecuteOrRequestApprovalAsync(
                directAction: async token => await _categoryService.DeleteAsync(id, token),
                buildApprovalRequest: () => new SubmitApprovalJobRequest
                {
                    Type = ApprovalJobType.CATEGORY,
                    Operation = Operation.DELETE,
                    TargetId = id,
                    Reason = CommonValidator.CategoryDeletionRequestedByEmployee(CurrentUserId, id)
                },
                approvalMessage: CommonValidator.CategoryDeletionValidationMessage,
                ct: ct,
                onSuccess: deleted => deleted ? NoContent() : NotFound()
            );
        }

        /// <summary>
        /// Executes the provided action directly if the current user is ADMIN; otherwise submits an approval job
        /// and returns <c>202 Accepted</c> with the job info.
        /// </summary>
        /// <typeparam name="TResult">The result type of the direct action.</typeparam>
        /// <param name="directAction">The action to execute immediately when the caller is ADMIN.</param>
        /// <param name="buildApprovalRequest">Factory to construct the approval job request for EMPLOYEE callers.</param>
        /// <param name="approvalMessage">A user-friendly message describing the approval process.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <param name="onSuccess">
        /// Optional custom formatter for the success result. If not provided, returns <c>200 OK</c> with the raw result.
        /// </param>
        /// <returns>
        /// Either the direct action result (<c>200/201/204</c>) when ADMIN, or <c>202 Accepted</c> with approval job details when EMPLOYEE.
        /// </returns>
        private async Task<IActionResult> ExecuteOrRequestApprovalAsync<TResult>(
            Func<CancellationToken, Task<TResult>> directAction,
            Func<SubmitApprovalJobRequest> buildApprovalRequest,
            string approvalMessage,
            CancellationToken ct,
            Func<TResult, IActionResult>? onSuccess = null)
        {
            if (IsAdmin)
            {
                var result = await directAction(ct);
                return onSuccess?.Invoke(result) ?? Ok(result!);
            }

            var job = await _approvalJobService.SubmitAsync(
                requesterUserId: CurrentUserId,
                buildApprovalRequest(),
                ct);

            return Accepted(new
            {
                message = approvalMessage,
                approvalJob = job
            });
        }
    }
}
