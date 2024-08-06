using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IClients;
using Models.UserManagement;

namespace Clients
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _javaBackendUrl = "http://localhost:8080";

        public AuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterUserAsync(RegisterUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_javaBackendUrl}/api/auth/register", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> LoginUserAsync(LoginUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_javaBackendUrl}/api/auth/login", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        
    }
}