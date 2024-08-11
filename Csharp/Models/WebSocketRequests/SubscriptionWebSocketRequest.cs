using System.Text.Json.Serialization;

namespace Models.WebSocketRequests
{
    public class SubscriptionWebSocketRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }  // The type of request (e.g., "subscribe", "unsubscribe", "getSubscribers")

        [JsonPropertyName("subscribedToId")]
        public long SubscribedToId { get; set; }  // The ID of the user to subscribe to or unsubscribe from

        [JsonPropertyName("userId")]
        public long UserId { get; set; }  // The ID of the user whose subscribers are to be retrieved
    }
}