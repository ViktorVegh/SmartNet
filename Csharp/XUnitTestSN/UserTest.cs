using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using IServices;
using Models;
using IClients;
using Services;

namespace XUnitTestSN
{
    public class UserTest
    {
        private readonly Mock<IUserClient> _userClientMock;
        private readonly Mock<ITokenHelper> _tokenHelperMock;
        private readonly IUserService _userService;

        public UserTest()
        {
            _userClientMock = new Mock<IUserClient>();
            _tokenHelperMock = new Mock<ITokenHelper>();
            _userService = new UserService(_userClientMock.Object, _tokenHelperMock.Object);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var username = "testuser";
            var expectedUser = new User { Username = username };
            _userClientMock.Setup(client => client.GetUserByUsernameAsync(username)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Equal(expectedUser, result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "nonexistentuser";
            _userClientMock.Setup(client => client.GetUserByUsernameAsync(username)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByTokenAsync_ShouldReturnUser_WhenTokenIsValid()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            var expectedUser = new User { Id = userId };
            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _userClientMock.Setup(client => client.GetUserByIdAsync(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByTokenAsync(token);

            // Assert
            Assert.Equal(expectedUser, result);
        }

        [Fact]
        public async Task GetUserByTokenAsync_ShouldReturnNull_WhenUserIdIsInvalid()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1L;
            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Returns(userId);
            _userClientMock.Setup(client => client.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByTokenAsync(token);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByTokenAsync_ShouldThrowException_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "invalid_token";
            _tokenHelperMock.Setup(helper => helper.GetUserIdFromToken(token)).Throws<ArgumentException>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByTokenAsync(token));
        }
    }
}
