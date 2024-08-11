using System.Text.Json.Serialization;

namespace Models.WebSocketRequests
{
    public class NotificationWebSocketRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}