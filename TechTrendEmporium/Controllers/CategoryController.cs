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
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IApprovalJobService _approvalJobService;

        public CategoryController(ICategoryService categoryService, IApprovalJobService approvalJobService)
        {
            _categoryService = categoryService;
            _approvalJobService = approvalJobService;
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE, SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _categoryService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

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

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest dto, CancellationToken ct)
        {
            var updated = await _categoryService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

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

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin => User.IsInRole("ADMIN");

        private async Task<IActionResult> ExecuteOrRequestApprovalAsync<TResult>(Func<CancellationToken, Task<TResult>> directAction, Func<SubmitApprovalJobRequest> buildApprovalRequest, string approvalMessage, CancellationToken ct, Func<TResult, IActionResult>? onSuccess = null)
        {
            if (IsAdmin)
            {
                var result = await directAction(ct);

                return onSuccess?.Invoke(result) ?? Ok(result!);
            }

            var job = await _approvalJobService.SubmitAsync(requesterUserId: CurrentUserId, buildApprovalRequest(), ct);

            return Accepted(new
            {
                message = approvalMessage,
                approvalJob = job
            });
        }
    }
}
