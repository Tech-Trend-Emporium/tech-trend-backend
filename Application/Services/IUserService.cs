using General.Dto.User;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing <see cref="User"/> entities and user-related business operations.
    /// This interface is documented by AI.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user based on the provided registration data.
        /// </summary>
        /// <param name="dto">The data transfer object containing user creation details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the newly created <see cref="UserResponse"/>.
        /// </returns>
        Task<UserResponse> CreateAsync(CreateUserRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="UserResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="UserResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<UserResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of users along with the total count of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a tuple with the list of <see cref="UserResponse"/> objects and the total count.
        /// </returns>
        Task<(IReadOnlyList<UserResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing user's information using the provided data.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="dto">The data transfer object containing the updated user information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the updated <see cref="UserResponse"/>.
        /// </returns>
        Task<UserResponse> UpdateAsync(int id, UpdateUserRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if the user was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a user with the specified username already exists.
        /// </summary>
        /// <param name="username">The username to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if a user with the specified username exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a user with the specified email address already exists.
        /// </summary>
        /// <param name="email">The email address to check for existence.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains <c>true</c> if a user with the specified email exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Counts the total number of users available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the total number of users.
        /// </returns>
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
