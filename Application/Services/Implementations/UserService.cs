using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.User;
using General.Mappers;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing users, including creation, update, deletion,
    /// and validation of uniqueness for username and email.
    /// This class is documented by AI.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">Repository for user data persistence and queries.</param>
        /// <param name="unitOfWork">Unit of Work for coordinating transactions and saving changes.</param>
        /// <param name="passwordHasher">Service used to securely hash user passwords.</param>
        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Returns the total number of users available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The total number of user records.</returns>
        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _userRepository.CountAsync(null, ct);
        }

        /// <summary>
        /// Creates a new user after validating that the username and email are unique.
        /// The password is securely hashed before saving.
        /// </summary>
        /// <param name="dto">The data transfer object containing user details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The created <see cref="UserResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="ConflictException">Thrown when the username or email already exists.</exception>
        public async Task<UserResponse> CreateAsync(CreateUserRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var normalizedUsername = (dto.Username ?? string.Empty).Trim().ToUpperInvariant();
            var normalizedEmail = (dto.Email ?? string.Empty).Trim().ToUpperInvariant();

            var usernameTaken = await _userRepository.ExistsAsync(u => u.Username.ToUpper() == normalizedUsername, ct);
            if (usernameTaken)
                throw new ConflictException(UserValidator.UserUsernameExists(dto.Username));

            var emailTaken = await _userRepository.ExistsAsync(u => u.Email.ToUpper() == normalizedEmail, ct);
            if (emailTaken)
                throw new ConflictException(UserValidator.UserEmailExists(dto.Email));

            var entity = UserMapper.ToEntity(dto);
            entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.Password);

            _userRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserMapper.ToResponse(entity);
        }

        /// <summary>
        /// Deletes a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if deleted successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _userRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Checks whether a user with the specified email already exists.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the email is already used; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized = (email ?? string.Empty).Trim().ToUpperInvariant();
            return _userRepository.ExistsAsync(u => u.Email.ToUpper() == normalized, ct);
        }

        /// <summary>
        /// Checks whether a user with the specified username already exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the username is already used; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        {
            var normalized = (username ?? string.Empty).Trim().ToUpperInvariant();
            return _userRepository.ExistsAsync(u => u.Username.ToUpper() == normalized, ct);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="UserResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _userRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            return UserMapper.ToResponse(entity);
        }

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A read-only list of <see cref="UserResponse"/> objects.</returns>
        public async Task<IReadOnlyList<UserResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _userRepository.ListAsync(skip, take, ct);
            return UserMapper.ToResponseList(entities);
        }

        /// <summary>
        /// Retrieves a paginated list of users along with the total count of available users.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Items</c>: a list of user responses.</description></item>
        /// <item><description><c>Total</c>: the total count of users.</description></item>
        /// </list>
        /// </returns>
        public async Task<(IReadOnlyList<UserResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _userRepository.ListAsync(skip, take, ct);
            var total = await _userRepository.CountAsync(null, ct);

            var items = UserMapper.ToResponseList(listTask);
            return (items, total);
        }

        /// <summary>
        /// Updates an existing user's data, validating that the username and email remain unique.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="dto">The update request data containing new user values.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="UserResponse"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
        /// <exception cref="ConflictException">Thrown when the updated username or email already exists.</exception>
        public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _userRepository.GetByIdAsync(ct, id);
            if (entity is null)
                throw new NotFoundException(UserValidator.UserNotFound(id));

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                var normalizedUsername = dto.Username.Trim().ToUpperInvariant();
                var usernameTaken = await _userRepository.ExistsAsync(u => u.Id != id && u.Username.ToUpper() == normalizedUsername, ct);

                if (usernameTaken)
                    throw new ConflictException(UserValidator.UserUsernameExists(dto.Username));
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var normalizedEmail = dto.Email.Trim().ToUpperInvariant();
                var emailTaken = await _userRepository.ExistsAsync(u => u.Id != id && u.Email.ToUpper() == normalizedEmail, ct);

                if (emailTaken)
                    throw new ConflictException(UserValidator.UserEmailExists(dto.Email));
            }

            UserMapper.ApplyUpdate(entity, dto);

            _userRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserMapper.ToResponse(entity);
        }
    }
}
