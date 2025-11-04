using Application.Dtos.Auth;
using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// API controller responsible for handling authentication and authorization operations,
    /// including user registration, sign-in, token refresh, and sign-out.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Gets the ID of the currently authenticated user from their claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service responsible for managing user authentication operations.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="dto">The user registration data including username, email, and password.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A newly created <see cref="SignUpResponse"/> representing the registered user.
        /// </returns>
        /// <response code="201">Returns the newly created user account.</response>
        /// <response code="400">If the request is invalid or missing required fields.</response>
        /// <response code="409">If the username or email is already in use.</response>
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(SignUpResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<SignUpResponse>> SignUp([FromBody] SignUpRequest dto, CancellationToken ct)
        {
            var result = await _authService.SignUp(dto, ct);
            return CreatedAtAction(nameof(SignUp), new { id = result.Id }, result);
        }

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <param name="dto">The user credentials used for sign-in.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="SignInResponse"/> containing authentication tokens and session details.
        /// </returns>
        /// <response code="200">Returns the access and refresh tokens for the authenticated user.</response>
        /// <response code="401">If the credentials are invalid or the account is inactive.</response>
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(SignInResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest dto, CancellationToken ct)
        {
            var result = await _authService.SignIn(dto, ct);
            return Ok(result);
        }

        /// <summary>
        /// Refreshes authentication tokens using a valid refresh token.
        /// </summary>
        /// <param name="dto">The refresh token request data containing the user's refresh token.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="SignInResponse"/> containing new access and refresh tokens.
        /// </returns>
        /// <response code="200">Returns new authentication tokens.</response>
        /// <response code="400">If the refresh token is missing or malformed.</response>
        /// <response code="401">If the refresh token is invalid or expired.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(SignInResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SignInResponse>> Refresh([FromBody] RefreshTokenRequest dto, CancellationToken ct)
        {
            var result = await _authService.RefreshToken(dto, ct);
            return Ok(result);
        }

        /// <summary>
        /// Signs out the currently authenticated user by revoking their tokens.
        /// </summary>
        /// <param name="dto">The sign-out request, which may specify whether to revoke all active sessions.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>No content if the sign-out process completes successfully.</returns>
        /// <response code="204">The user was successfully signed out.</response>
        /// <response code="400">If a specific sign-out is requested but no refresh token is provided.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [Authorize]
        [HttpPost("sign-out")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SignOut([FromBody] SignOutRequest dto, CancellationToken ct)
        {
            await _authService.SignOut(dto, CurrentUserId, ct);
            return NoContent();
        }

        /// <summary>
        /// Sets or updates the current user's recovery question and answer.
        /// The answer is normalized and stored as a hash server-side.
        /// </summary>
        /// <param name="dto">The recovery question id and plain-text answer.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Recovery info was saved successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Recovery question not found.</response>
        [Authorize]
        [HttpPost("recovery/set-info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SetRecoveryInfo([FromBody] SetRecoveryInfoRequest dto, CancellationToken ct)
        {
            await _authService.SetRecoveryInfoAsync(CurrentUserId, dto, ct);
            return NoContent();
        }

        /// <summary>
        /// Verifies a user's recovery answer and issues a short-lived password reset token if correct.
        /// </summary>
        /// <param name="dto">Email or username, recovery question id, and the answer.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A short-lived reset token to be used on the password reset endpoint.</returns>
        /// <response code="200">Returns a password reset token and its expiration.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="401">Recovery not configured or answer is incorrect.</response>
        /// <response code="404">User not found.</response>
        [AllowAnonymous]
        [HttpPost("recovery/verify")]
        [ProducesResponseType(typeof(VerifyRecoveryAnswerResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<VerifyRecoveryAnswerResponse>> VerifyRecovery([FromBody] VerifyRecoveryAnswerRequest dto, CancellationToken ct)
        {
            var result = await _authService.VerifyRecoveryAnswerAsync(dto, ct);
            return Ok(result);
        }

        /// <summary>
        /// Resets a user's password using a valid password reset token obtained from the verification endpoint.
        /// </summary>
        /// <param name="dto">The reset token and the new password.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Password was reset successfully.</response>
        /// <response code="400">Validation failed (e.g., password policy).</response>
        /// <response code="401">Token is invalid or expired.</response>
        /// <response code="404">User not found.</response>
        [AllowAnonymous]
        [HttpPost("recovery/reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto, CancellationToken ct)
        {
            await _authService.ResetPasswordAsync(dto, ct);
            return NoContent();
        }
    }
}
