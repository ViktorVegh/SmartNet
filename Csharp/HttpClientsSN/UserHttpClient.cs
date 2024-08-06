using IHttpClientsSN;
using Microsoft.Extensions.Configuration;
using Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpClientsSN
{
    public class UserHttpClient : IUserHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _webApiUrl;

        public UserHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _webApiUrl = "http://localhost:5276";
        }

        private JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters = { new ContentJsonConverter() }
            };
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_webApiUrl}/api/user/me");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent, GetJsonSerializerOptions());
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var response = await _httpClient.GetAsync($"{_webApiUrl}/api/user/username/{username}");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(responseContent, GetJsonSerializerOptions());
        }
    }
}
