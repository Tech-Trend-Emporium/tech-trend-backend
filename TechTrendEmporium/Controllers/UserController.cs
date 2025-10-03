using Application.Services;
using Asp.Versioning;
using General.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _userService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _userService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("exists/username")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByUsername([FromQuery] string username, CancellationToken ct)
        {
            var exists = await _userService.ExistsByUsernameAsync(username, ct);

            return Ok(new { Exists = exists });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("exists/email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsByEmail([FromQuery] string email, CancellationToken ct)
        {
            var exists = await _userService.ExistsByEmailAsync(email, ct);

            return Ok(new { Exists = exists });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest dto, CancellationToken ct)
        {
            var created = await _userService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto, CancellationToken ct)
        {
            var updated = await _userService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _userService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
