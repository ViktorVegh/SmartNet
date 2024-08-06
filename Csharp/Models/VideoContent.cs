namespace Models
{
    public class VideoContent : Content
    {
        public string VideoUrl { get; set; }
        public int Duration { get; set; }

        public VideoContent() 
        {
            Type = "video"; 
        }

        public VideoContent(string videoUrl, int duration)
        {
            VideoUrl = videoUrl;
            Duration = duration;
            Type = "video"; 
        }
    }
}