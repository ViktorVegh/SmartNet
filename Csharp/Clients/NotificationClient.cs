using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IClients;
using Models.DTOs;

namespace Clients
{
    public class NotificationClient : INotificationClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _javaBackendUrl = "http://localhost:8080";

        public NotificationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendNotificationAsync(long userId, string message)
        {
            var response = await _httpClient.PostAsync(
                $"{_javaBackendUrl}/api/notifications?userId={userId}&message={message}",
                null
            );
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_javaBackendUrl}/api/notifications/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<NotificationDto>>();
        }
    }
}