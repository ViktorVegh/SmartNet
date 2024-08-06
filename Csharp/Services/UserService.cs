using IClients;
using IServices;
using Models;
using System.Threading.Tasks;

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
            return await _userClient.GetUserByUsernameAsync(username);
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            var userId = _tokenHelper.GetUserIdFromToken(token);
            var user = await _userClient.GetUserByIdAsync(userId);
            return user;
        }
    }
}