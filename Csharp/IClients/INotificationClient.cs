using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;

namespace IClients
{
    public interface INotificationClient
    {
        Task SendNotificationAsync(long userId, string message);
        Task<List<NotificationDto>> GetNotificationsAsync(long userId);
    }
}