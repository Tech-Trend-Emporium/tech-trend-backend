using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using General.Dto.User;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.UserService
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut; // System Under Test

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser_WhenDataIsValid()
        {
            // Arrange
            var ct = CancellationToken.None;
            var existingUser = new User { Id = 1, Username = "OLDUSER", Email = "OLD@MAIL.COM", Role = Domain.Enums.Role.ADMIN };
            var dto = new UpdateUserRequest { Username = "NEWUSER", Email = "NEW@MAIL.COM", Role = Domain.Enums.Role.EMPLOYEE };

            _userRepository.GetByIdAsync(ct, existingUser.Id).Returns(existingUser);
            _userRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct).Returns(false);

            // Act
            var result = await _sut.UpdateAsync(existingUser.Id, dto, ct);

            // Assert            
            Assert.Equal(dto.Username, result.Username);
            Assert.Equal(dto.Email, result.Email);           
            _userRepository.Received(1).Update(existingUser);
            await _unitOfWork.Received(1).SaveChangesAsync(ct);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenDtoIsNull()
        {
            // Arrange
            var ct = CancellationToken.None;
            int id = 1;
            UpdateUserRequest? dto = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(id, dto, ct));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;            
            var dto = new UpdateUserRequest { Username = "Someone" };
            int id = 999;

            _userRepository.GetByIdAsync(ct, id).Returns((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await _sut.UpdateAsync(id, dto, ct));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflict_WhenUsernameAlreadyExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int id = 1;
            var existingUser = new User { Id = id, Username = "OLDUSER", Email = "OLD@MAIL.COM" };
            var dto = new UpdateUserRequest { Username = "TAKENUSER" };

            _userRepository.GetByIdAsync(ct, 1).Returns(existingUser);
            _userRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct).Returns(true); // Username conflict

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _sut.UpdateAsync(id, dto, ct));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            int id = 1;
            var existingUser = new User { Id = id, Username = "OLDUSER", Email = "OLD@MAIL.COM" };
            var dto = new UpdateUserRequest { Username = "NEWUSER", Email = "TAKEN@MAIL.COM" };

            _userRepository.GetByIdAsync(ct, id).Returns(existingUser);

            // Simulate: username check OK, email check FAIL
            _userRepository.ExistsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>(), ct).Returns(false, true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _sut.UpdateAsync(id, dto, ct));
        }
    }
}
