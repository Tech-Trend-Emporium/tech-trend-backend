using Application.Services;
using Asp.Versioning;
using General.Dto.Coupon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _couponService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CouponResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _couponService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCouponRequest dto, CancellationToken ct)
        {
            var created = await _couponService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(CouponResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponRequest dto, CancellationToken ct)
        {
            var updated = await _couponService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _couponService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
