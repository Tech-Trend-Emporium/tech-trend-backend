using Application.Services;
using Asp.Versioning;
using General.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing user accounts.
    /// Provides endpoints for creating, retrieving, updating, and deleting users,
    /// as well as checking for existing usernames and emails.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The service responsible for user-related operations.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with the user details; otherwise <c>404 Not Found</c>.</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">If the user does not exist.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var result = await _userService.GetByIdAsync(id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of users along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with <c>{ Total, Items }</c> representing the users and total count.</returns>
        /// <response code="200">Returns the paginated list of users.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var (items, total) = await _userService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }

        /// <summary>
        /// Checks whether a user exists with the specified username.
        /// </summary>
        /// <param name="username">The username to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with <c>{ Exists }</c> indicating whether the username exists.</returns>
        /// <response code="200">Returns the existence status of the username.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("exists/username")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByUsername([FromQuery] string username, CancellationToken ct)
        {
            var exists = await _userService.ExistsByUsernameAsync(username, ct);
            return Ok(new { Exists = exists });
        }

        /// <summary>
        /// Checks whether a user exists with the specified email address.
        /// </summary>
        /// <param name="email">The email address to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with <c>{ Exists }</c> indicating whether the email exists.</returns>
        /// <response code="200">Returns the existence status of the email.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("exists/email")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByEmail([FromQuery] string email, CancellationToken ct)
        {
            var exists = await _userService.ExistsByEmailAsync(email, ct);
            return Ok(new { Exists = exists });
        }

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="dto">The user creation payload containing username, email, and password.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>201 Created</c> with the created <see cref="UserResponse"/>.</returns>
        /// <response code="201">User created successfully.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="409">If the username or email already exists.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest dto, CancellationToken ct)
        {
            var created = await _userService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="dto">The update payload containing new user information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>200 OK</c> with the updated <see cref="UserResponse"/>.</returns>
        /// <response code="200">User updated successfully.</response>
        /// <response code="404">If the user does not exist.</response>
        /// <response code="409">If the username or email is already taken.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequest dto, CancellationToken ct)
        {
            var updated = await _userService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>204 No Content</c> if deleted successfully; otherwise <c>404 Not Found</c>.</returns>
        /// <response code="204">User deleted successfully.</response>
        /// <response code="404">If the user does not exist.</response>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            var deleted = await _userService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
