using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public abstract class Content
    { 
        public long Id { get; set; } 
        public long LearningMaterialId { get; set; }
        [Required]
        public string Type { get; set; } 
        [JsonIgnore] 
        public LearningMaterial LearningMaterial { get; set; }
    }
}