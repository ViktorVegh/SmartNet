using System;

namespace Models
{
    public class Subscription
    {
        public long Id { get; set; }
        public User Subscriber { get; set; }
        public User SubscribedTo { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}