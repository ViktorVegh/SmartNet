namespace Models
{
    public class PhotoContent : Content
    {
        public string ImageUrl { get; set; }

        public PhotoContent() 
        {
            Type = "photo"; 
        }

        public PhotoContent(string imageUrl)
        {
            ImageUrl = imageUrl;
            Type = "photo"; 
        }
    }

}