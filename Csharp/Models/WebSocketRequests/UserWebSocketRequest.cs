using System.Text.Json.Serialization;

namespace Models.WebSocketRequests
{
    public class UserWebSocketRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; } 
    }
}