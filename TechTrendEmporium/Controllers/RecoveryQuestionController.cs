using Application.Dtos.RecoveryQuestion;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller responsible for managing recovery questions, including creation, retrieval,
    /// listing, updating, and deletion operations.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class RecoveryQuestionController : ControllerBase
    {
        private readonly IRecoveryQuestionService _recoveryQuestionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryQuestionController"/> class.
        /// </summary>
        /// <param name="recoveryQuestionService">Service for managing recovery-question–related business logic.</param>
        public RecoveryQuestionController(IRecoveryQuestionService recoveryQuestionService)
        {
            _recoveryQuestionService = recoveryQuestionService;
        }

        /// <summary>
        /// Retrieves a recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the recovery question.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the <see cref="RecoveryQuestionResponse"/> if found; otherwise, <c>404 Not Found</c>.
        /// </returns>
        /// <response code="200">Returns the recovery question.</response>
        /// <response code="404">If the recovery question does not exist.</response>
        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RecoveryQuestionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _recoveryQuestionService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of recovery questions along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip before returning results. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an object containing the total count and list of <see cref="RecoveryQuestionResponse"/> objects.
        /// </returns>
        /// <response code="200">Returns the paginated list of recovery questions.</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _recoveryQuestionService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Creates a new recovery question.
        /// </summary>
        /// <param name="dto">The recovery question creation payload.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>201 Created</c> with the created <see cref="RecoveryQuestionResponse"/>.
        /// </returns>
        /// <response code="201">Returns the created recovery question.</response>
        /// <response code="400">If the request data is invalid.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(RecoveryQuestionResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateRecoveryQuestionRequest dto, CancellationToken ct)
        {
            var created = await _recoveryQuestionService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing recovery question.
        /// </summary>
        /// <param name="id">The recovery question identifier to update.</param>
        /// <param name="dto">The update payload containing new question data.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the updated <see cref="RecoveryQuestionResponse"/>.
        /// </returns>
        /// <response code="200">Returns the updated recovery question.</response>
        /// <response code="404">If the recovery question does not exist.</response>
        /// <response code="400">If the provided data is invalid.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RecoveryQuestionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRecoveryQuestionRequest dto, CancellationToken ct)
        {
            var updated = await _recoveryQuestionService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a recovery question by its unique identifier.
        /// </summary>
        /// <param name="id">The recovery question identifier to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>204 No Content</c> if deleted successfully; otherwise <c>404 Not Found</c>.
        /// </returns>
        /// <response code="204">Recovery question deleted successfully.</response>
        /// <response code="404">Recovery question not found.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var deleted = await _recoveryQuestionService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
