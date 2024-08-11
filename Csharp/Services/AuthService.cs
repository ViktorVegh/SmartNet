using System;
using System.Threading.Tasks;
using IClients;
using IServices;
using Models;
using Models.UserManagement;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthClient _authClient;
        private readonly IUserService _userService;

        public AuthService(IAuthClient authClient, IUserService userService)
        {
            _authClient = authClient;
            _userService = userService;
        }
        
        public async Task<string> RegisterUserAsync(RegisterUser user)
        {
           
            ValidateRegisterUser(user);

            try
            {
               
                var existingUser = await _userService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                  
                    throw new ArgumentException("Username already exists.", nameof(user.Username));
                }
            }
            catch (Exception ex)
            {
                
                if (ex.Message.Contains("User not found"))
                {
                   
                    return await _authClient.RegisterUserAsync(user);
                }
                else
                {
                  
                    throw;
                }
            }

            
            throw new ArgumentException("Username already exists.", nameof(user.Username));
        }


        public async Task<string> LoginUserAsync(LoginUser user)
        {
            ValidateLoginUser(user);
            var existingUser = await _userService.GetUserByUsernameAsync(user.Username);
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
