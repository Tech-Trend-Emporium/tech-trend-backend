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
    public class ListWithCountAsyncTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut;

        public ListWithCountAsyncTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnItemsAndTotal_WhenUsersExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            var users = new List<User>
            {
                new User { Id = 1, Username = "Alice", Email = "alice@example.com" },
                new User { Id = 2, Username = "Bob", Email = "bob@example.com" }
            };
            var totalCount = users.Count;

            var expectedResponses = UserMapper.ToResponseList(users);

            _userRepository.ListAsync(skip, take, ct).Returns(users);
            _userRepository.CountAsync(null, ct).Returns(totalCount);

            // Act
            var (items, total) = await _sut.ListWithCountAsync(skip, take, ct);

            // Assert
            Assert.Equal(totalCount, total);
            Assert.Equal(expectedResponses.Count, items.Count);
            Assert.Equal(expectedResponses[0].Username, items[0].Username);
            Assert.Equal(expectedResponses[1].Username, items[1].Username);
            await _userRepository.Received(1).ListAsync(skip, take, ct);
            await _userRepository.Received(1).CountAsync(null, ct);
        }

        [Fact]
        public async Task ListWithCountAsync_ShouldReturnEmptyListAndZero_WhenNoUsersExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            int skip = 0, take = 50;
            _userRepository.ListAsync(skip, take, ct).Returns(new List<User>());
            _userRepository.CountAsync(null, ct).Returns(0);

            // Act
            var (items, total) = await _sut.ListWithCountAsync(skip, take, ct);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
            Assert.Equal(0,total);
        }
    }
}
