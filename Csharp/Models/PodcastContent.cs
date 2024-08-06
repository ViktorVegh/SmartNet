namespace Models
{
    public class PodcastContent : Content
    {
        public string AudioUrl { get; set; }
        public int Duration { get; set; }

        public PodcastContent() 
        {
            Type = "podcast";
        }

        public PodcastContent(string audioUrl, int duration)
        {
            AudioUrl = audioUrl;
            Duration = duration;
            Type = "podcast"; 
        }
    }
}