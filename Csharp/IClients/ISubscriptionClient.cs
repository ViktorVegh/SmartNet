using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace IClients
{
    public interface ISubscriptionClient
    {
        Task SubscribeAsync(long subscriberId, long subscribedToId);
        Task UnsubscribeAsync(long subscriberId, long subscribedToId);
        Task<List<Subscription>> GetSubscribersAsync(long userId);
    }
}