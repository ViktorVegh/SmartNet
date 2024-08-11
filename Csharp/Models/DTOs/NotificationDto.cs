using System;
using Models.UserManagement;

namespace Models.DTOs
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}