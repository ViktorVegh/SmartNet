using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace IClients
{
    public interface IUserClient
    {
        Task<User> GetUserByIdAsync(long id);
        Task<User> GetUserByUsernameAsync(string username);
        
    }
}