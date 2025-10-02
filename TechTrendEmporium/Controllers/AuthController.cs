using Application.Dtos.Auth;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("sign-up")]
        public async Task<ActionResult<SignUpResponse>> SignUp([FromBody] SignUpRequest dto, CancellationToken ct)
        {
            var result = await _authService.SignUp(dto, ct);

            return CreatedAtAction(nameof(SignUp), new { id = result.Id }, result);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest dto, CancellationToken ct)
        {
            var result = await _authService.SignIn(dto, ct);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<SignInResponse>> Refresh([FromBody] RefreshTokenRequest dto, CancellationToken ct)
        {
            var result = await _authService.RefreshToken(dto, ct);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOut([FromBody] SignOutRequest dto, CancellationToken ct)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _authService.SignOut(dto, currentUserId, ct);
            
            return NoContent();
        }
    }
}
