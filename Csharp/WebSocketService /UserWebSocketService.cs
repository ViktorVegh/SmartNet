using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.UserManagement;
using IWebSocketService;

namespace WebSocketService
{
    public class UserWebSocketService : IUserWebSocketService, IAsyncDisposable
    {
        private ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<UserWebSocketService> _logger;
        private readonly string _webSocketUri;

        public UserWebSocketService(ILogger<UserWebSocketService> logger)
        {
            _webSocket = new ClientWebSocket();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webSocketUri = "ws://localhost:5276/ws/user";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new ContentJsonConverter() }
            };
        }

        public async Task ConnectAsync()
        {
            _logger.LogInformation("Connecting...");
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                if (_webSocket?.State == WebSocketState.Closed || _webSocket?.State == WebSocketState.Aborted)
                {
                    _webSocket?.Dispose();
                    _webSocket = new ClientWebSocket();
                }

                try
                {
                    await _webSocket.ConnectAsync(new Uri(_webSocketUri), CancellationToken.None);
                    _logger.LogInformation("Connected to WebSocket server at {Uri}", _webSocketUri);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to WebSocket server at {Uri}", _webSocketUri);
                    throw;
                }
            }
        }

        public async Task<User> GetUserByTokenAsync(string token)
        {
            var request = new
            {
                Type = "getUserByToken",
                Token = token
            };

            var responseJson = await SendMessageAsync(request);
            _logger.LogInformation("Got USER:{Message}",responseJson);
            Console.WriteLine($"Got user json: {responseJson}");
            
            var uu = JsonSerializer.Deserialize<User>(responseJson,_jsonOptions);
            Console.WriteLine($"Got3: {uu.Username}");
            return !string.IsNullOrEmpty(responseJson) ? uu : null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var request = new
            {
                Type = "getUserByUsername",
                Username = username
            };

            var responseJson = await SendMessageAsync(request);
            _logger.LogInformation("Got user by user name :{Message}",responseJson);
            return !string.IsNullOrEmpty(responseJson) ? JsonSerializer.Deserialize<User>(responseJson, _jsonOptions) : null;
        }

        public async Task<UserDto> GetUserDtoByIdAsync(long id)
        {
            var request = new
            {
                Type = "getUserDtoById",
                Id = id
            };

            var responseJson = await SendMessageAsync(request);
            _logger.LogInformation("Got user dto :{Message}",responseJson);
            return !string.IsNullOrEmpty(responseJson) ? JsonSerializer.Deserialize<UserDto>(responseJson, _jsonOptions) : null;
        }

        private async Task<string> SendMessageAsync<T>(T message)
        {
            try
            {
                await ConnectAsync(); 

                var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
                var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogInformation("Sent message to WebSocket server: {Message}", messageJson);

                var buffer = new byte[1024 * 4];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.Count == 0)
                {
                    _logger.LogWarning("Received an empty response from WebSocket server.");
                    return null;
                }

                var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("Received response from WebSocket server: {Response}", response);

                return response;
            }
            catch (WebSocketException wsex)
            {
                _logger.LogError(wsex, "WebSocket error while sending/receiving message.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending/receiving message to/from WebSocket server.");
                return null;
            }
        }

        public async Task CloseAsync()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _logger.LogInformation("WebSocket connection closed.");
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError(ex, "Error occurred while closing WebSocket connection.");
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
            _webSocket?.Dispose();
        }
    }
}
