using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.UserService
{
    public class ExistsByEmailTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut;

        public ExistsByEmailTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            string email = "test@example.com";
            _userRepository.ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(true);

            // Act
            var result = await _sut.ExistsByEmailAsync(email, ct);

            // Assert
            Assert.True(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            );
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            string email = "unknown@example.com";
            _userRepository.ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(false);

            // Act
            var result = await _sut.ExistsByEmailAsync(email, ct);

            // Assert
            Assert.False(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            );
        }

        [Fact]
        public async Task ExistsByEmailAsync_ShouldHandleNullEmail_Gracefully()
        {
            // Arrange
            var ct = CancellationToken.None;
            _userRepository.ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(false);

            // Act
            var result = await _sut.ExistsByEmailAsync(null, ct);

            // Assert
            Assert.False(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            );
        }
    }
}
