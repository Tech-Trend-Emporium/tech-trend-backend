using Application.Dtos.ApprovalJob;
using Application.Services;
using Asp.Versioning;
using Domain.Enums;
using Domain.Validations;
using General.Dto.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing products, including retrieval, listing with pagination and optional category filter,
    /// creation, partial update, and deletion. Employee-initiated create/delete operations are submitted as approval jobs.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IApprovalJobService _approvalJobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">Business service for product operations.</param>
        /// <param name="approvalJobService">Service to submit approval jobs for non-admin callers.</param>
        public ProductController(IProductService productService, IApprovalJobService approvalJobService)
        {
            _productService = productService;
            _approvalJobService = approvalJobService;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from JWT claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Indicates whether the current user has the ADMIN role.
        /// </summary>
        private bool IsAdmin => User.IsInRole("ADMIN");

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="ct">An optional cancellation token.</param>
        /// <returns><c>200 OK</c> with the product; otherwise <c>404 Not Found</c>.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product does not exist.</response>
        [HttpGet("by_id/{id:int}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _productService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByName([FromRoute] string name, CancellationToken ct)
        {
            var result = await _productService.GetByNameAsync(name, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of products along with the total count.
        /// Optionally filters by category name.
        /// </summary>
        /// <param name="skip">Records to skip (default 0).</param>
        /// <param name="take">Records to take (default 50).</param>
        /// <param name="category">Optional category name filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns><c>200 OK</c> with <c>{ Total, Items }</c>.</returns>
        /// <response code="200">Returns the page of products with total count.</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            [FromQuery] string? category = null,
            CancellationToken ct = default)
        {
            var (items, total) = await _productService.ListWithCountAsync(skip, take, category, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Creates a new product.
        /// Admins execute immediately; employees submit an approval job.
        /// </summary>
        /// <param name="dto">The product creation payload.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description><c>201 Created</c> with the created product (ADMIN).</description></item>
        /// <item><description><c>202 Accepted</c> with approval job details (EMPLOYEE).</description></item>
        /// </list>
        /// </returns>
        /// <response code="201">Product created successfully (ADMIN).</response>
        /// <response code="202">Approval requested (EMPLOYEE).</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Category not found.</response>
        /// <response code="409">Product already exists.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
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

        /// <summary>
        /// Partially updates an existing product.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="dto">The partial update payload.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns><c>200 OK</c> with the updated product.</returns>
        /// <response code="200">Product updated successfully.</response>
        /// <response code="404">Product or referenced category not found.</response>
        /// <response code="400">Validation error.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a product.
        /// Admins execute immediately; employees submit an approval job.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description><c>204 No Content</c> if deleted (ADMIN).</description></item>
        /// <item><description><c>404 Not Found</c> if the product does not exist (ADMIN path).</description></item>
        /// <item><description><c>202 Accepted</c> with approval job info (EMPLOYEE).</description></item>
        /// </list>
        /// </returns>
        /// <response code="204">Deleted successfully (ADMIN).</response>
        /// <response code="202">Approval requested (EMPLOYEE).</response>
        /// <response code="404">Product not found (ADMIN path).</response>
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

        /// <summary>
        /// Executes the provided action directly if the current user is ADMIN; otherwise submits an approval job
        /// and returns <c>202 Accepted</c> with the job info.
        /// </summary>
        /// <typeparam name="TResult">The result type of the direct action.</typeparam>
        /// <param name="directAction">The action to execute immediately when the caller is ADMIN.</param>
        /// <param name="buildApprovalRequest">Factory that constructs the approval job request for EMPLOYEE callers.</param>
        /// <param name="approvalMessage">A user-friendly message describing the approval process.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="onSuccess">Optional custom formatter for the success result; defaults to <c>200 OK</c>.</param>
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
