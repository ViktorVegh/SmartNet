using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IClients;
using Models;
using Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Clients
{
    public class LearningMaterialClient : ILearningMaterialClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LearningMaterialClient> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public LearningMaterialClient(HttpClient httpClient, ILogger<LearningMaterialClient> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8080/");
            _logger = logger;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial)
        {
            
            var jsonPayload = JsonSerializer.Serialize(learningMaterial, _jsonSerializerOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);
            
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            _logger.LogInformation("Sending POST request to {Url} with payload: {Payload}",
                _httpClient.BaseAddress + "api/learning-materials", jsonPayload);
            
            var response = await _httpClient.PostAsync("api/learning-materials", content);
            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            
            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, _jsonSerializerOptions);
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
        {
            _logger.LogInformation("Sending GET request to {Url}", $"api/learning-materials/{id}");
            var response = await _httpClient.GetAsync($"api/learning-materials/{id}");
            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            
            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, _jsonSerializerOptions);
        }

        public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
        {
            _logger.LogInformation("Sending GET request to {Url}", "api/learning-materials");
            var response = await _httpClient.GetAsync("api/learning-materials");
            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            
            return JsonSerializer.Deserialize<List<LearningMaterial>>(responseContent, _jsonSerializerOptions);
        }

        public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, CreateLearningMaterial learningMaterial)
        {
            var jsonPayload = JsonSerializer.Serialize(learningMaterial, _jsonSerializerOptions);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending PUT request to {Url} with payload: {Payload}", _httpClient.BaseAddress + $"api/learning-materials/{id}", jsonPayload);

            var response = await _httpClient.PutAsync($"api/learning-materials/{id}", content);
            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            
            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, _jsonSerializerOptions);
        }

        public async Task DeleteLearningMaterialAsync(long id)
        {
            _logger.LogInformation("Sending DELETE request to {Url}", $"api/learning-materials/{id}");
            var response = await _httpClient.DeleteAsync($"api/learning-materials/{id}");
            _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();
        }
    }
}
