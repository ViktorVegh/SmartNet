using System;

namespace Models
{
    public class Notification
    {
        public long Id { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}