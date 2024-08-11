// using IHttpClientsSN;
// using Microsoft.Extensions.Configuration;
// using Models;
// using System.Net.Http;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
// using System.Text.Json;
// using System.Text.Json.Serialization;
//
// namespace HttpClientsSN
// {
//     public class UserHttpClient : IUserHttpClient
//     {
//         private readonly HttpClient _httpClient;
//         private readonly string _webApiUrl;
//
//         public UserHttpClient(HttpClient httpClient, IConfiguration configuration)
//         {
//             _httpClient = httpClient;
//             _webApiUrl = "http://localhost:5276";
//         }
//
//         private JsonSerializerOptions GetJsonSerializerOptions()
//         {
//             return new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//                 DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//                 WriteIndented = true,
//                 Converters = { new ContentJsonConverter() }
//             };
//         }
//
//         public async Task<User> GetUserByTokenAsync(string token)
//         {
//             _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//             var response = await _httpClient.GetAsync($"{_webApiUrl}/api/user/me");
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             return JsonSerializer.Deserialize<User>(responseContent, GetJsonSerializerOptions());
//         }
//
//         public async Task<User> GetUserByUsernameAsync(string username)
//         {
//             var response = await _httpClient.GetAsync($"{_webApiUrl}/api/user/username/{username}");
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             return JsonSerializer.Deserialize<User>(responseContent, GetJsonSerializerOptions());
//         }
//     }
// }

using IHttpClientsSN;
using Microsoft.Extensions.Configuration;
using Models;
using Models.DTOs;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientsSN
{
    public class UserHttpClient : IUserHttpClient
    {
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _webSocketUri;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserHttpClient(IConfiguration configuration)
        {
            _webSocket = new ClientWebSocket();
            _webSocketUri = new Uri("ws://localhost:5276/ws/user"); // WebSocket endpoint for user operations
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new ContentJsonConverter() }
            };
        }

        private async Task EnsureConnectedAsync()
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(_webSocketUri, CancellationToken.None);
            }
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "getUserByToken"
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            return JsonSerializer.Deserialize<User>(responseJson, _jsonOptions);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "getUserByUsername",
                Username = username
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            return JsonSerializer.Deserialize<User>(responseJson, _jsonOptions);
        }

        public async Task<UserDto> GetUserDtoByIdAsync(long id)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "getUserDtoById",
                Id = id
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            return JsonSerializer.Deserialize<UserDto>(responseJson, _jsonOptions); // Using default serialization for UserDto
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
