using Models;
using System.Threading.Tasks;

namespace IHttpClientsSN
{
    public interface IUserHttpClient
    {
        Task<User> GetUserByTokenAsync(string token);
        Task<User> GetUserByUsernameAsync(string username);
    }
}