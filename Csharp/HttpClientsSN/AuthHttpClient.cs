// using IHttpClientsSN;
// using Microsoft.Extensions.Configuration;
// using Models;
// using Models.UserManagement;
//
// namespace HttpClientsSN;
//
// using System.Net.Http;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
//
//
//     public class AuthHttpClient : IAuthHttpClient
//     {
//         private readonly HttpClient _httpClient;
//         private readonly string _webApiUrl;
//
//         public AuthHttpClient(HttpClient httpClient, IConfiguration configuration)
//         {
//             _httpClient = httpClient;
//             _webApiUrl = "http://localhost:5276";
//         }
//
//         public async Task<string> RegisterUserAsync(RegisterUser user)
//         {
//             var response = await _httpClient.PostAsJsonAsync($"{_webApiUrl}/api/auth/register", user);
//             response.EnsureSuccessStatusCode();
//             return await response.Content.ReadAsStringAsync();
//         }
//
//         public async Task<string> LoginUserAsync(LoginUser user)
//         {
//             var response = await _httpClient.PostAsJsonAsync($"{_webApiUrl}/api/auth/login", user);
//             response.EnsureSuccessStatusCode();
//             return await response.Content.ReadAsStringAsync();
//         }
//         
//     }
//


using IHttpClientsSN;
using Microsoft.Extensions.Configuration;
using Models.UserManagement;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientsSN
{
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _webSocketUri;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _webSocket = new ClientWebSocket();
            _webSocketUri = new Uri("ws://localhost:5276/ws/auth"); // WebSocket endpoint for auth
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private async Task EnsureConnectedAsync()
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(_webSocketUri, CancellationToken.None);
            }
        }

        public async Task<string> RegisterUserAsync(RegisterUser user)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "register",
                RegisterUser = user
            };

            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            return responseJson;
        }

        public async Task<string> LoginUserAsync(LoginUser user)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "login",
                LoginUser = user
            };

            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            return responseJson;
        }

        public async ValueTask DisposeAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            _webSocket.Dispose();
        }
    }
}
