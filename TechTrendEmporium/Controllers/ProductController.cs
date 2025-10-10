using Application.Abstraction;
using Application.Dtos.ApprovalJob;
using Application.Services;
using Asp.Versioning;
using Domain.Enums;
using Domain.Validations;
using General.Dto.Product;
using General.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IApprovalJobService _approvalJobService;

        public ProductController(IProductService productService, IApprovalJobService approvalJobService)
        {
            _productService = productService;
            _approvalJobService = approvalJobService;
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _productService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, [FromQuery] string? category = null, CancellationToken ct = default)
        {
            var (items, total) = await _productService.ListWithCountAsync(skip, take, category, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest dto, CancellationToken ct)
        {
            return await ExecuteOrRequestApprovalAsync(
                directAction: token => _productService.CreateAsync(dto, token),
                buildApprovalRequest: () => new SubmitApprovalJobRequest
                {
                    Type = ApprovalJobType.PRODUCT,
                    Operation = Operation.CREATE,
                    Payload = dto,
                    Reason = CommonValidator.ProductCreationRequestedByEmployee(CurrentUserId)
                },
                approvalMessage: CommonValidator.ProductCreationValidationMessage,
                ct: ct,
                onSuccess: created => CreatedAtAction(nameof(GetById), new { id = created!.Id }, created!)
            );
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            return await ExecuteOrRequestApprovalAsync(
                directAction: token => _productService.DeleteAsync(id, token),
                buildApprovalRequest: () => new SubmitApprovalJobRequest
                {
                    Type = ApprovalJobType.PRODUCT,
                    Operation = Operation.DELETE,
                    TargetId = id,
                    Reason = CommonValidator.ProductDeletionRequestedByEmployee(CurrentUserId, id)
                },
                approvalMessage: CommonValidator.ProductDeletionValidationMessage,
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
