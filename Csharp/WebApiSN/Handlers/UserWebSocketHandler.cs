using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Http;
using Models;
using Models.WebSocketRequests;

namespace WebApiSN.Handlers
{
    public class UserWebSocketHandler
    {
        private readonly IUserService _userService;
        private readonly JsonSerializerOptions _serializerOptions;
        
        public UserWebSocketHandler(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new ContentJsonConverter() }
            };
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var response = await ProcessMessage(message);

                var responseMessage = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task<string> ProcessMessage(string message)
        {
            var request = JsonSerializer.Deserialize<UserWebSocketRequest>(message, _serializerOptions);

            try
            {
                switch (request.Type)
                {
                    case "getUserByToken":
                        var user = await _userService.GetUserByTokenAsync(request.Token);
                        return user != null 
                            ? JsonSerializer.Serialize(user, _serializerOptions) 
                            : JsonSerializer.Serialize(new { Error = "User not found" });

                    case "getUserByUsername":
                        user = await _userService.GetUserByUsernameAsync(request.Username);
                        return user != null 
                            ? JsonSerializer.Serialize(user , _serializerOptions) 
                            : JsonSerializer.Serialize(new { Error = "User not found" });

                    case "getUserDtoById":
                        var userDto = await _userService.GetUserDtoByIdAsync(request.Id);
                        return userDto != null
                            ? JsonSerializer.Serialize(new { UserDto = userDto }, _serializerOptions)
                            : JsonSerializer.Serialize(new { Error = "User not found" });

                    default:
                        return JsonSerializer.Serialize(new { Error = "Invalid request type" });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Error = "An unexpected error occurred", Details = ex.Message });
            }
        }
    }
}
