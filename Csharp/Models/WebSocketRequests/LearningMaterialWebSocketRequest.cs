using System.Text.Json.Serialization;
using Models.DTOs;

namespace Models.WebSocketRequests
{
    public class LearningMaterialWebSocketRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("headlineSubstring")]
        public string HeadlineSubstring { get; set; }

        [JsonPropertyName("createLearningMaterial")]
        public CreateLearningMaterial CreateLearningMaterial { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; } 
    }
}