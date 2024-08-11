using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IClients;
using Models;

namespace Clients
{
    public class SubscriptionClient : ISubscriptionClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _javaBackendUrl = "http://localhost:8080";

        public SubscriptionClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SubscribeAsync(long subscriberId, long subscribedToId)
        {
            var response = await _httpClient.PostAsync(
                $"{_javaBackendUrl}/api/subscriptions/subscribe?subscriberId={subscriberId}&subscribedToId={subscribedToId}",
                null
            );
            response.EnsureSuccessStatusCode();
        }

        public async Task UnsubscribeAsync(long subscriberId, long subscribedToId)
        {
            var response = await _httpClient.PostAsync(
                $"{_javaBackendUrl}/api/subscriptions/unsubscribe?subscriberId={subscriberId}&subscribedToId={subscribedToId}",
                null
            );
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Subscription>> GetSubscribersAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_javaBackendUrl}/api/subscriptions/subscribers/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Subscription>>();
        }
    }
}