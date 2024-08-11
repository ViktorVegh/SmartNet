using Models;
using Models.DTOs;

namespace IWebSocketService;

public interface IUserWebSocketService
{
    Task ConnectAsync();
    Task<User> GetUserByTokenAsync(string token);
    Task<User> GetUserByUsernameAsync(string username);
    Task<UserDto> GetUserDtoByIdAsync(long id);
    Task CloseAsync();
}