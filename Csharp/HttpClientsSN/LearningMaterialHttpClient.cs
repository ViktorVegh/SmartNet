using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IHttpClientsSN;
using Models;
using Models.DTOs;
using Microsoft.Extensions.Logging;

namespace HttpClientsSN
{
    public class LearningMaterialHttpClient : ILearningMaterialHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LearningMaterialHttpClient> _logger;
        private readonly string _webApiUrl = "http://localhost:5276/api/LearningMaterials";

        public LearningMaterialHttpClient(HttpClient httpClient, ILogger<LearningMaterialHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters = { new ContentJsonConverter() }
            };
        }

        public async Task<LearningMaterial> AddLearningMaterialAsync(CreateLearningMaterial learningMaterial, string token)
        {
            SetAuthorizationHeader(token);
            var options = GetJsonSerializerOptions();

            var jsonPayload = JsonSerializer.Serialize(learningMaterial, options);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_webApiUrl, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);

            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, options);
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(long id)
        {
            var response = await _httpClient.GetAsync($"{_webApiUrl}/{id}");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, GetJsonSerializerOptions());
        }

        public async Task<List<LearningMaterial>> GetAllLearningMaterialsAsync()
        {
            var response = await _httpClient.GetAsync(_webApiUrl);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);
            return JsonSerializer.Deserialize<List<LearningMaterial>>(responseContent, GetJsonSerializerOptions());
        }

        public async Task<LearningMaterial> UpdateLearningMaterialAsync(long id, LearningMaterial learningMaterial, string token)
        {
            SetAuthorizationHeader(token);
            var options = GetJsonSerializerOptions();

            var jsonPayload = JsonSerializer.Serialize(learningMaterial, options);
            _logger.LogInformation("Serialized JSON Payload: {JsonPayload}", jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_webApiUrl}/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response JSON: {ResponseContent}", responseContent);

            return JsonSerializer.Deserialize<LearningMaterial>(responseContent, options);
        }

        public async Task DeleteLearningMaterialAsync(long id, string token)
        {
            SetAuthorizationHeader(token);
            var response = await _httpClient.DeleteAsync($"{_webApiUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        private void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
