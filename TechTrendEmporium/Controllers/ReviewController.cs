using Application.Services;
using Asp.Versioning;
using General.Dto.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing product reviews. 
    /// Supports creation, retrieval, updating, deletion, and listing (with pagination and filtering by product).
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewController"/> class.
        /// </summary>
        /// <param name="reviewService">The service responsible for review management operations.</param>
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Retrieves a specific review by its unique identifier.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the review if found; otherwise <c>404 Not Found</c>.
        /// </returns>
        /// <response code="200">Returns the review.</response>
        /// <response code="404">If the review does not exist.</response>
        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _reviewService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of all reviews along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an object containing <c>Total</c> and <c>Items</c> (a list of reviews).
        /// </returns>
        /// <response code="200">Returns the list of reviews with total count.</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var (items, total) = await _reviewService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Retrieves a paginated list of reviews for a specific product along with the total count.
        /// </summary>
        /// <param name="productId">The product identifier for which to retrieve reviews.</param>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an object containing <c>Total</c> and <c>Items</c> (the list of reviews for the product).
        /// </returns>
        /// <response code="200">Returns the list of reviews for the specified product.</response>
        /// <response code="404">If the product does not exist.</response>
        [Authorize]
        [HttpGet("product/{productId:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListByProduct(
            [FromRoute] int productId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var (items, total) = await _reviewService.ListByProductWithCountAsync(productId, skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Creates a new review for a product.
        /// </summary>
        /// <param name="dto">The review creation request containing username, product, rating, and comment.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>201 Created</c> with the created <see cref="ReviewResponse"/>.
        /// </returns>
        /// <response code="201">Review created successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="404">If the user or product is not found.</response>
        /// <response code="409">If a review by the same user already exists for the product.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest dto, CancellationToken ct)
        {
            var created = await _reviewService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing review by ID.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="dto">The review update request containing updated rating and/or comment.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with the updated review.</returns>
        /// <response code="200">Review updated successfully.</response>
        /// <response code="404">If the review or associated user does not exist.</response>
        [Authorize]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateReviewRequest dto, CancellationToken ct)
        {
            var updated = await _reviewService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a review by its unique identifier.
        /// </summary>
        /// <param name="id">The review identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>204 No Content</c> if deleted successfully; otherwise <c>404 Not Found</c>.
        /// </returns>
        /// <response code="204">Review deleted successfully.</response>
        /// <response code="404">Review not found.</response>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var deleted = await _reviewService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
