using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);

        Task<User> GetUserByTokenAsync(string token);
    }
}