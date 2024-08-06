using System;
using System.Collections.Generic;

namespace Models
{
    public class LearningMaterial
    {
        public long Id { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public bool MembersOnly { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long UserId { get; set; }
        public ICollection<Content> Contents { get; set; }

        public LearningMaterial()
        {
            Contents = new List<Content>();
        }

        public LearningMaterial(string headline, string description, bool membersOnly, long userId)
        {
            Headline = headline;
            Description = description;
            MembersOnly = membersOnly;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UserId = userId;
            Contents = new List<Content>();
        }
    }
}