using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IWebSocketService;
using Models;
using Microsoft.Extensions.Logging;
using Models.DTOs;

namespace WebSocketService
{
    public class LearningMaterialWebSocketService : ILearningMaterialWebSocketService, IAsyncDisposable
    {
        private ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<LearningMaterialWebSocketService> _logger;
        private readonly string _webSocketUri = "ws://localhost:5276/ws/learning-material";

        public LearningMaterialWebSocketService(ILogger<LearningMaterialWebSocketService> logger)
        {
            _webSocket = new ClientWebSocket();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new ContentJsonConverter() }
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

        public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token)
        {
            await ConnectAsync(); 

            var request = new
            {
                Type = "add",
                CreateLearningMaterial = learningMaterial,
                Token = token 
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
        {
            await ConnectAsync();

            var request = new
            {
                Type = "getById",
                Id = id
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
        {
            await ConnectAsync(); 

            var request = new
            {
                type = "getAll"
            };

            var responseJson = await SendMessageAsync(request);
            _logger.LogInformation("Received JSON: {responseJson}");

            if (string.IsNullOrEmpty(responseJson))
            {
                _logger.LogWarning("Received empty JSON response.");
                return null;
            }

            try
            {
                var learningMaterials = JsonSerializer.Deserialize<List<LearningMaterial>>(responseJson, _jsonOptions);
                _logger.LogInformation("Deserialized LearningMaterials Count: {Count}", learningMaterials?.Count ?? 0);
                return learningMaterials;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing LearningMaterials");
                return null;
            }
        }



        public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token)
        {
            await ConnectAsync(); 

            var createLearningMaterial = TransformToCreateLearningMaterial(learningMaterial);

            var request = new
            {
                Type = "update",
                Id = id,
                CreateLearningMaterial = createLearningMaterial,
                Token = token 
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        private CreateLearningMaterial TransformToCreateLearningMaterial(LearningMaterial learningMaterial)
        {
            return new CreateLearningMaterial
            {
                Headline = learningMaterial.Headline,
                Description = learningMaterial.Description,
                UserId = learningMaterial.UserId,
                Contents = learningMaterial.Contents.ToList()
            };
        }


        public async Task<string> DeleteLearningMaterialAsync(long id, string token)
        {
            await ConnectAsync();

            var request = new
            {
                Type = "delete",
                Id = id,
                Token = token 
            };

            return await SendMessageAsync(request); 
        }

        public async Task<List<LearningMaterial>> GetLearningMaterialsByHeadlineAsync(string headlineSubstring)
        {
            await ConnectAsync(); 

            var request = new
            {
                Type = "getByHeadline",
                HeadlineSubstring = headlineSubstring
            };

            var responseJson = await SendMessageAsync(request);
            return JsonSerializer.Deserialize<List<LearningMaterial>>(responseJson, _jsonOptions);
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
