using IClients;
using IServices;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IUserClient _userClient;
        private readonly ITokenHelper _tokenHelper;
        private readonly INotificationService _notificationService;

        public SubscriptionService(
            ISubscriptionClient subscriptionClient,
            IUserClient userClient,
            ITokenHelper tokenHelper,
            INotificationService notificationService)
        {
            _subscriptionClient = subscriptionClient;
            _userClient = userClient;
            _tokenHelper = tokenHelper;
            _notificationService = notificationService;
        }

        public async Task SubscribeAsync(string token, long subscribedToId)
        {
            var subscriberId = _tokenHelper.GetUserIdFromToken(token);

            if (subscriberId <= 0)
            {
                throw new ArgumentException("Invalid subscriber ID", nameof(subscriberId));
            }

            if (subscribedToId <= 0)
            {
                throw new ArgumentException("Invalid subscribed to ID", nameof(subscribedToId));
            }

            try
            {
                await _subscriptionClient.SubscribeAsync(subscriberId, subscribedToId);

                var subscriber = await _userClient.GetUserByIdAsync(subscriberId);
                var message = $"{subscriber.Username} started subscribing to you.";

                await _notificationService.SendNotificationAsync(subscribedToId, message);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while subscribing: {ex.Message}", ex);
            }
        }

        public async Task UnsubscribeAsync(string token, long subscribedToId)
        {
            var subscriberId = _tokenHelper.GetUserIdFromToken(token);

            if (subscriberId <= 0)
            {
                throw new ArgumentException("Invalid subscriber ID", nameof(subscriberId));
            }

            if (subscribedToId <= 0)
            {
                throw new ArgumentException("Invalid subscribed to ID", nameof(subscribedToId));
            }

            try
            {
                await _subscriptionClient.UnsubscribeAsync(subscriberId, subscribedToId);

                var subscriber = await _userClient.GetUserByIdAsync(subscriberId);
                var message = $"{subscriber.Username} stopped subscribing to you.";

                await _notificationService.SendNotificationAsync(subscribedToId, message);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while unsubscribing: {ex.Message}", ex);
            }
        }

        public async Task<List<Subscription>> GetSubscribersAsync(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            try
            {
                return await _subscriptionClient.GetSubscribersAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving subscribers: {ex.Message}", ex);
            }
        }
    }
}

