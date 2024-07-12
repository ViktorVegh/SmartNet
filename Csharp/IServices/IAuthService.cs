using Models;

namespace IServices;

public interface IAuthService
{
    Task<string> RegisterUserAsync(RegisterUser user);
    Task<string> LoginUserAsync(LoginUser user);
}