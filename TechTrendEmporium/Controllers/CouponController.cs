using Application.Services;
using Asp.Versioning;
using General.Dto.Coupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller responsible for managing coupons, including creation, retrieval,
    /// listing, updating, and deletion operations.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponController"/> class.
        /// </summary>
        /// <param name="couponService">Service for managing coupon-related business logic.</param>
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Retrieves a coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the coupon.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the <see cref="CouponResponse"/> if found; otherwise, <c>404 Not Found</c>.
        /// </returns>
        /// <response code="200">Returns the coupon.</response>
        /// <response code="404">If the coupon does not exist.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _couponService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of coupons along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip before returning results. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an object containing the total count and list of <see cref="CouponResponse"/> objects.
        /// </returns>
        /// <response code="200">Returns the paginated list of coupons.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _couponService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Creates a new coupon.
        /// </summary>
        /// <param name="dto">The coupon creation payload.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>201 Created</c> with the created <see cref="CouponResponse"/>.
        /// </returns>
        /// <response code="201">Returns the created coupon.</response>
        /// <response code="400">If the request data is invalid.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCouponRequest dto, CancellationToken ct)
        {
            var created = await _couponService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing coupon.
        /// </summary>
        /// <param name="id">The coupon identifier to update.</param>
        /// <param name="dto">The update payload containing new coupon data.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the updated <see cref="CouponResponse"/>.
        /// </returns>
        /// <response code="200">Returns the updated coupon.</response>
        /// <response code="404">If the coupon does not exist.</response>
        /// <response code="400">If the provided data is invalid.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCouponRequest dto, CancellationToken ct)
        {
            var updated = await _couponService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a coupon by its unique identifier.
        /// </summary>
        /// <param name="id">The coupon identifier to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>204 No Content</c> if deleted successfully; otherwise <c>404 Not Found</c>.
        /// </returns>
        /// <response code="204">Coupon deleted successfully.</response>
        /// <response code="404">Coupon not found.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var deleted = await _couponService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
