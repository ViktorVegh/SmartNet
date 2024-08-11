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
    public class SubscriptionWebSocketHandler
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly JsonSerializerOptions _serializerOptions;

        public SubscriptionWebSocketHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
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
            var request = JsonSerializer.Deserialize<SubscriptionWebSocketRequest>(message, _serializerOptions);

            try
            {
                switch (request.Type)
                {
                    case "subscribe":
                        var token = ExtractTokenFromHeaders(context);
                        await _subscriptionService.SubscribeAsync(token, request.SubscribedToId);
                        return JsonSerializer.Serialize(new { Message = "Subscribed successfully" }, _serializerOptions);

                    case "unsubscribe":
                        token = ExtractTokenFromHeaders(context);
                        await _subscriptionService.UnsubscribeAsync(token, request.SubscribedToId);
                        return JsonSerializer.Serialize(new { Message = "Unsubscribed successfully" }, _serializerOptions);

                    case "getSubscribers":
                        var subscribers = await _subscriptionService.GetSubscribersAsync(request.UserId);
                        return JsonSerializer.Serialize(new { Subscribers = subscribers }, _serializerOptions);

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

        private string ExtractTokenFromHeaders(HttpContext context)
        {
            return context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }
    }
}
