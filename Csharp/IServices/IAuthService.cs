using Models;
using Models.UserManagement;

namespace IServices;

public interface IAuthService
{
    Task<string> RegisterUserAsync(RegisterUser user);
    Task<string> LoginUserAsync(LoginUser user);
}