using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using Models.WebSocketRequests;

namespace WebApiSN.Handlers
{
    public class LearningMaterialWebSocketHandler
    {
        private readonly ILearningMaterialService _learningMaterialService;
        private readonly ILogger<LearningMaterialWebSocketHandler> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public LearningMaterialWebSocketHandler(ILearningMaterialService learningMaterialService, ILogger<LearningMaterialWebSocketHandler> logger)
        {
            _learningMaterialService = learningMaterialService ?? throw new ArgumentNullException(nameof(learningMaterialService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                var response = await ProcessMessage(context, message);

                var responseMessage = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task<string> ProcessMessage(HttpContext context, string message)
        {
            var request = JsonSerializer.Deserialize<LearningMaterialWebSocketRequest>(message, _serializerOptions);

            try
            {
                var token = request.Token;

                switch (request.Type)
                {
                    case "add":
                        var createdLearningMaterial = await _learningMaterialService.AddLearningMaterialAsync(request.CreateLearningMaterial, token);
                        return JsonSerializer.Serialize(new { LearningMaterial = createdLearningMaterial }, _serializerOptions);

                    case "getById":
                        var learningMaterial = await _learningMaterialService.GetLearningMaterialByIdAsync(request.Id);
                        return learningMaterial != null 
                            ? JsonSerializer.Serialize(new { LearningMaterial = learningMaterial }, _serializerOptions) 
                            : JsonSerializer.Serialize(new { Error = "Learning material not found" });

                    case "getAll":
                        var materials = await _learningMaterialService.GetAllLearningMaterialsAsync();
                        return JsonSerializer.Serialize(new { LearningMaterials = materials }, _serializerOptions);

                    case "update":
                        var updatedLearningMaterial = await _learningMaterialService.UpdateLearningMaterialAsync(request.Id, request.CreateLearningMaterial, token);
                        return JsonSerializer.Serialize(new { LearningMaterial = updatedLearningMaterial }, _serializerOptions);

                    case "delete":
                        await _learningMaterialService.DeleteLearningMaterialAsync(request.Id, token);
                        return JsonSerializer.Serialize(new { Message = "Learning material deleted successfully" });
                    
                    case "getByHeadline":
                        var filteredMaterials = await _learningMaterialService.GetLearningMaterialsByHeadlineAsync(request.HeadlineSubstring);
                        return JsonSerializer.Serialize(new { LearningMaterials = filteredMaterials }, _serializerOptions);

                    default:
                        return JsonSerializer.Serialize(new { Error = "Invalid request type" }, _serializerOptions);
                }
            }
            catch (ArgumentException ex)
            {
                return JsonSerializer.Serialize(new { Error = ex.Message }, _serializerOptions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return JsonSerializer.Serialize(new { Error = "Unauthorized access", Details = ex.Message }, _serializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return JsonSerializer.Serialize(new { Error = "An unexpected error occurred", Details = ex.Message }, _serializerOptions);
            }
        }
    }
}
