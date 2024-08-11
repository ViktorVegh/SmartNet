using Models;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);

        Task<User> GetUserByTokenAsync(string token);
        
        Task<UserDto> GetUserDtoByIdAsync(long id);
    }
}