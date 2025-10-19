using Application.Dtos.Auth;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for authentication and authorization operations within the application layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user account based on the provided sign-up information.
        /// </summary>
        /// <param name="dto">The data transfer object containing the user's registration details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignUpResponse"/> with information about the newly registered user.
        /// </returns>
        Task<SignUpResponse> SignUp(SignUpRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Authenticates a user and generates access and refresh tokens upon successful sign-in.
        /// </summary>
        /// <param name="dto">The data transfer object containing the user's sign-in credentials.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignInResponse"/> with authentication tokens and user details.
        /// </returns>
        Task<SignInResponse> SignIn(SignInRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Refreshes the user's authentication tokens using a valid refresh token.
        /// </summary>
        /// <param name="dto">The data transfer object containing the refresh token information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignInResponse"/> with new authentication tokens.
        /// </returns>
        Task<SignInResponse> RefreshToken(RefreshTokenRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Signs out the current user by revoking active authentication tokens.
        /// </summary>
        /// <param name="dto">The data transfer object containing sign-out details.</param>
        /// <param name="currentUserId">The unique identifier of the user performing the sign-out action.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SignOut(SignOutRequest dto, int currentUserId, CancellationToken ct = default);
    }
}
