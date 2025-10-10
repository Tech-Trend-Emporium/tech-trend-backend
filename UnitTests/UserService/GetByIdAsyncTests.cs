using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
using General.Dto.User;
using General.Mappers;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.UserService
{
    public class GetByIdAsyncTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut;

        public GetByIdAsyncTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUserResponse_WhenUserExists()
        {
            // Arrange
            var id = 1;
            var ct = CancellationToken.None;
            var userEntity = new User
            {
                Id = id,
                Username = "Kevin",
                Email = "kevin@example.com",
                PasswordHash = "hashedPassword",
                Role = Domain.Enums.Role.EMPLOYEE,

            };

            var expectedResponse = UserMapper.ToResponse(userEntity);
            _userRepository.GetByIdAsync(ct, id)
                .Returns(userEntity);

            // Act
            var result = await _sut.GetByIdAsync(id, ct);

            // Assert
            Assert.Equal(expectedResponse.Id, result.Id);
            Assert.Equal(expectedResponse.Username, result.Username);
            Assert.Equal(expectedResponse.Email, result.Email);            
            await _userRepository.Received(1).GetByIdAsync(ct, id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var id = 999;
            var ct = CancellationToken.None;

            _userRepository.GetByIdAsync(ct, id).Returns((User?)null);

            // Act
            var result = await _sut.GetByIdAsync(id, ct);

            // Assert
            Assert.Null(result);
            await _userRepository.Received(1).GetByIdAsync(ct, id);
        }
    }
}
