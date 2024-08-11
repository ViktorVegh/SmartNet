using System.Text.Json.Serialization;
using Models.UserManagement;

namespace Models.WebSocketRequests
{
    public class AuthWebSocketRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("registerUser")]
        public RegisterUser RegisterUser { get; set; }

        [JsonPropertyName("loginUser")]
        public LoginUser LoginUser { get; set; }
    }
}