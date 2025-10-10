using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
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
    public class ListAsyncTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut;

        public ListAsyncTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnMappedResponses_WhenUsersExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var users = new List<User>
            {
                new User { Id = 1, Username = "Alice", Email = "alice@example.com" },
                new User { Id = 2, Username = "Bob", Email = "bob@example.com" }
            };

            var expectedResponses = UserMapper.ToResponseList(users);

            _userRepository.ListAsync(skip, take, ct).Returns(users);

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponses.Count, result.Count);
            Assert.Equal(expectedResponses[0].Username, result[0].Username);
            Assert.Equal(expectedResponses[1].Username, result[1].Username);
            await _userRepository.Received(1).ListAsync(0, 50, ct);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;

            _userRepository.ListAsync(skip, take, ct).Returns(new List<User>());

            // Act
            var result = await _sut.ListAsync(skip, take, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _userRepository.Received(1).ListAsync(skip, take, ct);
        }
    }
}
