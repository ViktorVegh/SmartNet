using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(long userId, string message);
        Task<List<NotificationDto>> GetNotificationsAsync(long userId);
    }
}