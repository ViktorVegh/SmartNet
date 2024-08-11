// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Net.Http.Json;
// using System.Text;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Threading.Tasks;
// using IHttpClientsSN;
// using Models;
// using Models.DTOs;
// using Microsoft.Extensions.Logging;
//
// namespace HttpClientsSN
// {
//     public class LearningMaterialHttpClient : ILearningMaterialHttpClient
//     {
//         private readonly HttpClient _httpClient;
//         private readonly ILogger<LearningMaterialHttpClient> _logger;
//         private readonly string _webApiUrl = "http://localhost:5276/api/LearningMaterials";
//
//         public LearningMaterialHttpClient(HttpClient httpClient, ILogger<LearningMaterialHttpClient> logger)
//         {
//             _httpClient = httpClient;
//             _logger = logger;
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
//         public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token)
//         {
//             SetAuthorizationHeader(token);
//             var options = GetJsonSerializerOptions();
//
//             var jsonPayload = JsonSerializer.Serialize(learningMaterial, options);
//             _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);
//
//             var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
//
//             var response = await _httpClient.PostAsync(_webApiUrl, content);
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
//
//             return JsonSerializer.Deserialize<LearningMaterial>(responseContent, options);
//         }
//
//         public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
//         {
//             var response = await _httpClient.GetAsync($"{_webApiUrl}/{id}");
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
//             return JsonSerializer.Deserialize<LearningMaterial>(responseContent, GetJsonSerializerOptions());
//         }
//
//         public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
//         {
//             var response = await _httpClient.GetAsync(_webApiUrl);
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
//             return JsonSerializer.Deserialize<List<LearningMaterial>>(responseContent, GetJsonSerializerOptions());
//         }
//
//         public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token)
//         {
//             SetAuthorizationHeader(token);
//             var options = GetJsonSerializerOptions();
//
//             var jsonPayload = JsonSerializer.Serialize(learningMaterial, options);
//             _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);
//
//             var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
//
//             var response = await _httpClient.PutAsync($"{_webApiUrl}/{id}", content);
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
//
//             return JsonSerializer.Deserialize<LearningMaterial>(responseContent, options);
//         }
//
//         public async Task DeleteLearningMaterialAsync(long id, string token)
//         {
//             SetAuthorizationHeader(token);
//             var response = await _httpClient.DeleteAsync($"{_webApiUrl}/{id}");
//             response.EnsureSuccessStatusCode();
//         }
//
//         private void SetAuthorizationHeader(string token)
//         {
//             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
//         }
//     }
// }


using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using IHttpClientsSN;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;

namespace HttpClientsSN
{
    public class LearningMaterialHttpClient : ILearningMaterialHttpClient
    {
        private readonly ClientWebSocket _webSocket;
        private readonly ILogger<LearningMaterialHttpClient> _logger;
        private readonly Uri _webSocketUri;
        private readonly JsonSerializerOptions _jsonOptions;

        public LearningMaterialHttpClient(ILogger<LearningMaterialHttpClient> logger)
        {
            _webSocket = new ClientWebSocket();
            _logger = logger;
            _webSocketUri = new Uri("ws://localhost:5276/ws/learning-material");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
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

        public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "add",
                CreateLearningMaterial = learningMaterial
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            _logger.LogInformation("Response JSON: {ResponseJson}", responseJson);

            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "getById",
                Id = id
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            _logger.LogInformation("Response JSON: {ResponseJson}", responseJson);

            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "getAll"
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            _logger.LogInformation("Response JSON: {ResponseJson}", responseJson);

            return JsonSerializer.Deserialize<List<LearningMaterial>>(responseJson, _jsonOptions);
        }

        public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "update",
                Id = id,
                CreateLearningMaterial = learningMaterial
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024 * 4];
            var responseSegment = new ArraySegment<byte>(responseBuffer);
            var response = await _webSocket.ReceiveAsync(responseSegment, CancellationToken.None);

            var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, response.Count);
            _logger.LogInformation("Response JSON: {ResponseJson}", responseJson);

            return JsonSerializer.Deserialize<LearningMaterial>(responseJson, _jsonOptions);
        }

        public async Task DeleteLearningMaterialAsync(long id, string token)
        {
            await EnsureConnectedAsync();

            var request = new
            {
                Type = "delete",
                Id = id
            };

            var jsonPayload = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var requestBytes = Encoding.UTF8.GetBytes(jsonPayload);
            var requestSegment = new ArraySegment<byte>(requestBytes);

            await _webSocket.SendAsync(requestSegment, WebSocketMessageType.Text, true, CancellationToken.None);

            // No need to deserialize response for delete action
            _logger.LogInformation("Delete action completed successfully.");
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
