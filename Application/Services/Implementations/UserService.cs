using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.User;
using General.Mappers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _userRepository.CountAsync(null, ct);
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var normalizedUsername = (dto.Username ?? string.Empty).Trim().ToUpperInvariant();
            var normalizedEmail = (dto.Email ?? string.Empty).Trim().ToUpperInvariant();

            var usernameTaken = await _userRepository.ExistsAsync(u => u.Username.ToUpper() == normalizedUsername, ct);
            if (usernameTaken) throw new ConflictException(UserValidator.UserUsernameExists(dto.Username));

            var emailTaken = await _userRepository.ExistsAsync(u => u.Email.ToUpper() == normalizedEmail, ct);
            if (emailTaken) throw new ConflictException(UserValidator.UserEmailExists(dto.Email));

            var entity = UserMapper.ToEntity(dto);
            entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.Password);

            _userRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserMapper.ToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _userRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized = (email ?? string.Empty).Trim().ToUpperInvariant();

            return _userRepository.ExistsAsync(u => u.Email.ToUpper() == normalized, ct);
        }

        public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        {
            var normalized = (username ?? string.Empty).Trim().ToUpperInvariant();

            return _userRepository.ExistsAsync(u => u.Username.ToUpper() == normalized, ct);
        }

        public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _userRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            return UserMapper.ToResponse(entity);
        }

        public async Task<IReadOnlyList<UserResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _userRepository.ListAsync(skip, take, ct);

            return UserMapper.ToResponseList(entities);
        }

        public async Task<(IReadOnlyList<UserResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _userRepository.ListAsync(skip, take, ct);
            var total = await _userRepository.CountAsync(null, ct);

            var items = UserMapper.ToResponseList(listTask);

            return (items, total);
        }

        public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _userRepository.GetByIdAsync(ct, id);
            if (entity is null) throw new NotFoundException(UserValidator.UserNotFound(id));

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                var normalizedUsername = dto.Username.Trim().ToUpperInvariant();
                var usernameTaken = await _userRepository.ExistsAsync(u => u.Id != id && u.Username.ToUpper() == normalizedUsername, ct);

                if (usernameTaken) throw new ConflictException(UserValidator.UserUsernameExists(dto.Username));
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var normalizedEmail = dto.Email.Trim().ToUpperInvariant();
                var emailTaken = await _userRepository.ExistsAsync(u => u.Id != id && u.Email.ToUpper() == normalizedEmail, ct);

                if (emailTaken) throw new ConflictException(UserValidator.UserEmailExists(dto.Email));
            }

            UserMapper.ApplyUpdate(entity, dto);

            _userRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return UserMapper.ToResponse(entity);
        }
    }
}
