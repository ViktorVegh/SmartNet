namespace IWebSocketService
{
    using System.Threading.Tasks;
    using Models.UserManagement;

    public interface IAuthWebSocketService
    {
        Task ConnectAsync();
        Task<AuthResponse> RegisterUserAsync(RegisterUser registerUser);
        Task<AuthResponse> LoginUserAsync(LoginUser loginUser);
        Task CloseAsync();
    }
}