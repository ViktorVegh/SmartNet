using IClients;
using IServices;
using Models.UserManagement;
using System;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserClient _userClient;
        private readonly ITokenHelper _tokenHelper;

        public UserService(IUserClient userClient, ITokenHelper tokenHelper)
        {
            _userClient = userClient;
            _tokenHelper = tokenHelper;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            var user = await _userClient.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            }

            var userId = _tokenHelper.GetUserIdFromToken(token);
            var user = await _userClient.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task<UserDto> GetUserDtoByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(id));
            }

            var userDto = await _userClient.GetUserDtoByIdAsync(id);
            if (userDto == null)
            {
                throw new Exception("User not found");
            }

            return userDto;
        }
    }
}