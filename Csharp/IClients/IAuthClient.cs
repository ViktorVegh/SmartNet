using System.Threading.Tasks;
using Models;
using Models.UserManagement;

namespace IClients
{
    public interface IAuthClient
    {
        Task<string> RegisterUserAsync(RegisterUser user);
        Task<string> LoginUserAsync(LoginUser user);
        
    }
}

