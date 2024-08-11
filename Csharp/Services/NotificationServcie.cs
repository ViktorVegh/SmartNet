using IClients;
using IServices;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationClient _notificationClient;

        public NotificationService(INotificationClient notificationClient)
        {
            _notificationClient = notificationClient;
        }

        public async Task SendNotificationAsync(long userId, string message)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            }

            try
            {
                await _notificationClient.SendNotificationAsync(userId, message);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while sending the notification: {ex.Message}", ex);
            }
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            try
            {
                var notifications = await _notificationClient.GetNotificationsAsync(userId);
                if (notifications == null || notifications.Count == 0)
                {
                    throw new Exception("No notifications found");
                }

                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving notifications: {ex.Message}", ex);
            }
        }
    }
}