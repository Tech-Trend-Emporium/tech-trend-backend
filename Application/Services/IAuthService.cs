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
        /// Optionally stores a recovery question and answer if supplied.
        /// </summary>
        /// <param name="dto">
        /// The data transfer object containing the user's registration details such as username,
        /// email, password, and optionally a recovery question and answer.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignUpResponse"/> with information about the newly registered user.
        /// </returns>
        Task<SignUpResponse> SignUp(SignUpRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Authenticates a user and generates access and refresh tokens upon successful sign-in.
        /// </summary>
        /// <param name="dto">The data transfer object containing the user's sign-in credentials.</param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignInResponse"/> with authentication tokens and user details.
        /// </returns>
        Task<SignInResponse> SignIn(SignInRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Refreshes the user's authentication tokens using a valid refresh token.
        /// </summary>
        /// <param name="dto">The data transfer object containing the refresh token information.</param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="SignInResponse"/> with new authentication tokens.
        /// </returns>
        Task<SignInResponse> RefreshToken(RefreshTokenRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Sets or updates the recovery question and hashed answer for the specified user.
        /// </summary>
        /// <param name="currentUserId">
        /// The unique identifier of the authenticated user whose recovery information is being configured.
        /// </param>
        /// <param name="dto">
        /// The data transfer object containing the recovery question identifier and the plain-text answer.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task completes when the recovery information has been persisted.
        /// </returns>
        Task SetRecoveryInfoAsync(int currentUserId, SetRecoveryInfoRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Validates a user’s recovery question and answer combination and issues
        /// a short-lived password reset token if the response is correct.
        /// </summary>
        /// <param name="dto">
        /// The data transfer object containing the user's email or username, the recovery question identifier,
        /// and the provided answer.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a <see cref="VerifyRecoveryAnswerResponse"/> with a temporary password reset token
        /// and its expiration timestamp.
        /// </returns>
        Task<VerifyRecoveryAnswerResponse> VerifyRecoveryAnswerAsync(VerifyRecoveryAnswerRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Resets the user’s password after validating the provided password reset token.
        /// This operation also updates the user's security stamp and revokes any active refresh tokens.
        /// </summary>
        /// <param name="dto">
        /// The data transfer object containing the password reset token and the new password to set.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task completes once the password has been successfully reset.
        /// </returns>
        Task ResetPasswordAsync(ResetPasswordRequest dto, CancellationToken ct = default);

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
