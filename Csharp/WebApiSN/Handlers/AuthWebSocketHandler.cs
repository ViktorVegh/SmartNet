using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models.WebSocketRequests;

namespace WebApiSN.Handlers
{
    public class AuthWebSocketHandler
    {
        private readonly IAuthService _authService;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<AuthWebSocketHandler> _logger;

        public AuthWebSocketHandler(IAuthService authService, ILogger<AuthWebSocketHandler> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            _logger.LogInformation("WebSocket connection established");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("Received WebSocket message: {Message}", message);

                var response = await ProcessMessage(context, message);

                var responseMessage = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                _logger.LogInformation("Sent WebSocket response: {Response}", response);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.LogInformation("WebSocket connection closed with status {Status}", result.CloseStatus);
        }

        private async Task<string> ProcessMessage(HttpContext context, string message)
        {
            try
            {
                _logger.LogInformation("Processing message: {Message}", message);
                
                var request = JsonSerializer.Deserialize<AuthWebSocketRequest>(message, _serializerOptions);

                if (request == null)
                {
                    _logger.LogWarning("Invalid request format: {Message}", message);
                    return JsonSerializer.Serialize(new { Error = "Invalid request format" }, _serializerOptions);
                }

                switch (request.Type)
                {
                    case "register":
                        _logger.LogInformation("Processing registration for user: {Username}", request.RegisterUser?.Username);
                        if (request.RegisterUser == null)
                        {
                            _logger.LogWarning("Missing RegisterUser data in request");
                            return JsonSerializer.Serialize(new { Error = "Missing RegisterUser data" }, _serializerOptions);
                        }

                        var token = await _authService.RegisterUserAsync(request.RegisterUser);
                        _logger.LogInformation("User registered successfully, token generated");
                        return JsonSerializer.Serialize(new { Token = token }, _serializerOptions);

                    case "login":
                        _logger.LogInformation("Processing login for user: {Username}", request.LoginUser?.Username);
                        if (request.LoginUser == null)
                        {
                            _logger.LogWarning("Missing LoginUser data in request");
                            return JsonSerializer.Serialize(new { Error = "Missing LoginUser data" }, _serializerOptions);
                        }

                        var loginToken = await _authService.LoginUserAsync(request.LoginUser);
                        if (loginToken != null)
                        {
                            _logger.LogInformation("User logged in successfully, token generated");
                            return JsonSerializer.Serialize(new { Token = loginToken }, _serializerOptions);
                        }
                        else
                        {
                            _logger.LogWarning("Invalid username or password for user: {Username}", request.LoginUser.Username);
                            return JsonSerializer.Serialize(new { Error = "Invalid username or password" }, _serializerOptions);
                        }

                    default:
                        _logger.LogWarning("Invalid request type: {Type}", request.Type);
                        return JsonSerializer.Serialize(new { Error = "Invalid request type" }, _serializerOptions);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format");
                return JsonSerializer.Serialize(new { Error = "Invalid JSON format", Details = ex.Message }, _serializerOptions);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred");
                return JsonSerializer.Serialize(new { Error = ex.Message }, _serializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return JsonSerializer.Serialize(new { Error = "An unexpected error occurred", Details = ex.Message }, _serializerOptions);
            }
        }
    }
}
