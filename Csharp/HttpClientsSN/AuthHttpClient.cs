using IHttpClientsSN;
using Microsoft.Extensions.Configuration;
using Models;

namespace HttpClientsSN;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _webApiUrl;

        public AuthHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _webApiUrl = "http://localhost:5276";
        }

        public async Task<string> RegisterUserAsync(RegisterUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_webApiUrl}/api/auth/register", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> LoginUserAsync(LoginUser user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_webApiUrl}/api/auth/login", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        
    }

