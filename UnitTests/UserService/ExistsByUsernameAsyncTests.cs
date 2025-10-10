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
    public class ExistsByUsernameAsyncTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Application.Services.Implementations.UserService _sut;

        public ExistsByUsernameAsyncTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _passwordHasher = Substitute.For<IPasswordHasher<User>>();

            _sut = new Application.Services.Implementations.UserService(_userRepository, _unitOfWork, _passwordHasher);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldReturnTrue_WhenUsernameExists()
        {
            // Arrange
            var ct = CancellationToken.None;
            string username = "Kevin";
            _userRepository
                .ExistsAsync(
                    Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                    ct)
                .Returns(true);

            // Act
            var result = await _sut.ExistsByUsernameAsync(username, CancellationToken.None);

            // Assert
            Assert.True(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                ct);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
        {
            // Arrange
            var ct = CancellationToken.None;
            string username = "UnknownUser";
            _userRepository
                .ExistsAsync(
                    Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                    ct)
                .Returns(false);

            // Act
            var result = await _sut.ExistsByUsernameAsync(username, ct);

            // Assert
            Assert.False(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                ct);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldHandleNullUsername_Gracefully()
        {
            // Arrange
            var ct = CancellationToken.None;
            _userRepository
                .ExistsAsync(
                    Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                    ct)
                .Returns(false);

            // Act
            var result = await _sut.ExistsByUsernameAsync(null, ct);

            // Assert
            Assert.False(result);
            await _userRepository.Received(1).ExistsAsync(
                Arg.Any<System.Linq.Expressions.Expression<System.Func<User, bool>>>(),
                ct);
        }

        [Fact]
        public async Task ExistsByUsernameAsync_ShouldNormalizeUsername_BeforeChecking()
        {
            // Arrange
            var ct = CancellationToken.None;
            string username = "   kevin   ";
            _userRepository
                .ExistsAsync(
                    Arg.Is<System.Linq.Expressions.Expression<System.Func<User, bool>>>(expr =>
                        expr.Compile().Invoke(new User { Username = "KEVIN" })),
                    ct)
                .Returns(true);

            // Act
            var result = await _sut.ExistsByUsernameAsync(username, ct);

            // Assert
            Assert.True(result);
        }
    }
}
