using Application.Services;
using Application.Services.Implementations;
using Asp.Versioning;
using General.Dto.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _reviewService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ReviewResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _reviewService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize]
        [HttpGet("product/{productId:int}")]
        [ProducesResponseType(typeof(IReadOnlyList<ReviewResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListByProduct(int productId, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _reviewService.ListByProductWithCountAsync(productId, skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest dto, CancellationToken ct)
        {
            var created = await _reviewService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequest dto, CancellationToken ct)
        {
            var updated = await _reviewService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _reviewService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
