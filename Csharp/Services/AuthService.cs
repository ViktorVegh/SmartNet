using System;
using System.Threading.Tasks;
using IClients;
using IServices;
using Models;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthClient _authClient;

        public AuthService(IAuthClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<string> RegisterUserAsync(RegisterUser user)
        {
            ValidateRegisterUser(user);

            
            var existingUser = await _authClient.GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new ArgumentException("Username already exists.", nameof(user.Username));
            }

            return await _authClient.RegisterUserAsync(user);
        }

        public async Task<string> LoginUserAsync(LoginUser user)
        {
            ValidateLoginUser(user);
            var existingUser = await _authClient.GetUserByUsernameAsync(user.Username);
            if (existingUser == null)
            {
                throw new ArgumentException("Username does not exist.", nameof(user.Username));
            }

            try
            {
                return await _authClient.LoginUserAsync(user);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new ArgumentException("Invalid username or password.", nameof(user));
            }
        }

        private void ValidateRegisterUser(RegisterUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be empty.", nameof(user.Username));

            if (user.Username.Length < 2 || user.Username.Length > 15)
                throw new ArgumentException("Username must be between 2 and 15 characters.", nameof(user.Username));

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(user.PasswordHash));

            if (user.PasswordHash.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(user.PasswordHash));

            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                throw new ArgumentException("Invalid email address.", nameof(user.Email));

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("First name cannot be empty.", nameof(user.FirstName));

            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(user.LastName));

            if (string.IsNullOrWhiteSpace(user.ProfilePicture) || !Uri.IsWellFormedUriString(user.ProfilePicture, UriKind.Absolute))
                throw new ArgumentException("Invalid profile picture URL.", nameof(user.ProfilePicture));
        }

        private void ValidateLoginUser(LoginUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be empty.", nameof(user.Username));

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(user.PasswordHash));

            if (user.PasswordHash.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(user.PasswordHash));
        }
    }
}
