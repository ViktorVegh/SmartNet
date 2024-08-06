using Models.UserManagement;

namespace IHttpClientsSN;

public interface IAuthHttpClient
{
    Task<string> RegisterUserAsync(RegisterUser user);
    Task<string> LoginUserAsync(LoginUser user);
}