using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IWebSocketService;
using Models.UserManagement;
using Microsoft.Extensions.Logging;

namespace WebSocketService
{
    public class AuthWebSocketService : IAuthWebSocketService, IAsyncDisposable
    {
        private ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<AuthWebSocketService> _logger;
        private readonly string _webSocketUri;

        public AuthWebSocketService(ILogger<AuthWebSocketService> logger)
        {
            _webSocket = new ClientWebSocket();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webSocketUri = "ws://localhost:5276/ws/auth";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task ConnectAsync()
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.Aborted)
                {
                    _webSocket.Dispose();
                    _webSocket = new ClientWebSocket();
                }
                
                await _webSocket.ConnectAsync(new Uri(_webSocketUri), CancellationToken.None);
                _logger.LogInformation("Connected to WebSocket server at {Uri}", _webSocketUri);
            }
        }

        public async Task<AuthResponse> RegisterUserAsync(RegisterUser registerUser)
        {
            await ConnectAsync(); 

            var request = new
            {
                Type = "register",
                RegisterUser = registerUser
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<AuthResponse>(responseJson, _jsonOptions);
        }

        public async Task<AuthResponse> LoginUserAsync(LoginUser loginUser)
        {
            await ConnectAsync();

            var request = new
            {
                Type = "login",
                LoginUser = loginUser
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<AuthResponse>(responseJson, _jsonOptions);
        }

        private async Task<string> SendMessageAsync<T>(T message)
        {
            try
            {
                var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
                var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogInformation("Sent message to WebSocket server: {Message}", messageJson);

                var buffer = new byte[1024 * 4];
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("Received response from WebSocket server: {Response}", response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending/receiving message to/from WebSocket server.");
                throw;
            }
        }

        public async Task CloseAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _logger.LogInformation("WebSocket connection closed.");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
            _webSocket.Dispose();
        }
    }
}
