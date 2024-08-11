using System.Text.Json.Serialization;

namespace Models.DTOs

{
    public class UserDto
    {
        [JsonPropertyName("id")] 
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("profilePicture")]
        public string ProfilePicture { get; set; }
    }
}