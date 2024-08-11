using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Models.DTOs;

namespace IClients
{
    public interface IUserClient
    {
        Task<User> GetUserByIdAsync(long id);
        Task<User> GetUserByUsernameAsync(string username);
        
        Task<UserDto> GetUserDtoByIdAsync(long id);
        
    }
}