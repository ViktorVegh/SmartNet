using System;
using System.Collections.Generic;

namespace Models.DTOs
{
    public class CreateLearningMaterial
    {
        public string Headline { get; set; }
        public string Description { get; set; }
        public bool MembersOnly { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Content> Contents { get; set; }

        public CreateLearningMaterial()
        {
            Contents = new List<Content>();
        }

        public CreateLearningMaterial(string headline, string description, bool membersOnly, long userId,
            DateTime createdAt, DateTime updatedAt, List<Content> contents)
        {
            Headline = headline;
            Description = description;
            MembersOnly = membersOnly;
            UserId = userId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Contents = contents ?? new List<Content>();
        }
    }
}