
namespace XUnitTestSN
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthClient> _authClientMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authClientMock = new Mock<IAuthClient>();
            _authService = new AuthService(_authClientMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerUser = new RegisterUser
            {
                Username = "testuser",
                PasswordHash = "password123",
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                ProfilePicture = "http://example.com/pic.jpg"
            };

            var token = "test_token";
            _authClientMock.Setup(client => client.GetUserByUsernameAsync(registerUser.Username)).ReturnsAsync((User)null);
            _authClientMock.Setup(client => client.RegisterUserAsync(registerUser)).ReturnsAsync(token);

            // Act
            var result = await _authService.RegisterUserAsync(registerUser);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            // Arrange
            var registerUser = new RegisterUser
            {
                Username = "existinguser",
                PasswordHash = "password123",
                Email = "existinguser@example.com",
                FirstName = "Existing",
                LastName = "User",
                ProfilePicture = "http://example.com/pic.jpg"
            };

            var existingUser = new User { Username = "existinguser" };
            _authClientMock.Setup(client => client.GetUserByUsernameAsync(registerUser.Username)).ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterUserAsync(registerUser));
            Assert.Equal("Username already exists. (Parameter 'Username')", exception.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginUser = new LoginUser
            {
                Username = "testuser",
                PasswordHash = "password123"
            };

            var token = "test_token";
            var existingUser = new User { Username = "testuser", PasswordHash = "password123" };
            _authClientMock.Setup(client => client.GetUserByUsernameAsync(loginUser.Username)).ReturnsAsync(existingUser);
            _authClientMock.Setup(client => client.LoginUserAsync(loginUser)).ReturnsAsync(token);

            // Act
            var result = await _authService.LoginUserAsync(loginUser);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldThrowException_WhenUsernameDoesNotExist()
        {
            // Arrange
            var loginUser = new LoginUser
            {
                Username = "nonexistentuser",
                PasswordHash = "password123"
            };

            _authClientMock.Setup(client => client.GetUserByUsernameAsync(loginUser.Username)).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginUserAsync(loginUser));
            Assert.Equal("Username does not exist. (Parameter 'Username')", exception.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginUser = new LoginUser
            {
                Username = "testuser",
                PasswordHash = "wrongpassword"
            };

            var existingUser = new User { Username = "testuser", PasswordHash = "correctpassword" };
            _authClientMock.Setup(client => client.GetUserByUsernameAsync(loginUser.Username)).ReturnsAsync(existingUser);
            _authClientMock.Setup(client => client.LoginUserAsync(loginUser)).Throws(new HttpRequestException(null, null, System.Net.HttpStatusCode.Unauthorized));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginUserAsync(loginUser));
            Assert.Equal("Invalid username or password. (Parameter 'user')", exception.Message);
        }
    }
}
