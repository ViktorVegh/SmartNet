using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Http;
using Models.WebSocketRequests;

namespace WebApiSN.Handlers
{
    public class NotificationWebSocketHandler
    {
        private readonly INotificationService _notificationService;
        private readonly JsonSerializerOptions _serializerOptions;

        public NotificationWebSocketHandler(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var response = await ProcessMessage(context, message);

                var responseMessage = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task<string> ProcessMessage(HttpContext context, string message)
        {
            var request = JsonSerializer.Deserialize<NotificationWebSocketRequest>(message, _serializerOptions);

            try
            {
                switch (request.Type)
                {
                    case "sendNotification":
                        await _notificationService.SendNotificationAsync(request.UserId, request.Message);
                        return JsonSerializer.Serialize(new { Message = "Notification sent successfully" }, _serializerOptions);

                    case "getNotifications":
                        var notifications = await _notificationService.GetNotificationsAsync(request.UserId);
                        return JsonSerializer.Serialize(new { Notifications = notifications }, _serializerOptions);

                    default:
                        return JsonSerializer.Serialize(new { Error = "Invalid request type" }, _serializerOptions);
                }
            }
            catch (ArgumentException ex)
            {
                return JsonSerializer.Serialize(new { Error = ex.Message }, _serializerOptions);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Error = "An unexpected error occurred", Details = ex.Message }, _serializerOptions);
            }
        }
    }
}
