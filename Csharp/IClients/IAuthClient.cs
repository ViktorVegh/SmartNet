using System.Threading.Tasks;
using Models;

namespace IClients
{
    public interface IAuthClient
    {
        Task<string> RegisterUserAsync(RegisterUser user);
        Task<string> LoginUserAsync(LoginUser user);
        Task<User> GetUserByUsernameAsync(string username);
    }
}

