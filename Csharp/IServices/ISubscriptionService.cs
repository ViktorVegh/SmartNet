using Models;

namespace IServices;

public interface ISubscriptionService
{
    Task SubscribeAsync(string token, long subscribedToId);
    Task UnsubscribeAsync(string token, long subscribedToId);
    Task<List<Subscription>> GetSubscribersAsync(long userId);
}