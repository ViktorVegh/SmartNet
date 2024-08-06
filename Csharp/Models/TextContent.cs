namespace Models
{
    public class TextContent : Content
    {
        public string Body { get; set; }

        public TextContent() 
        {
            Type = "text"; 
        }

        public TextContent(string body)
        {
            Body = body;
            Type = "text"; 
        }
    }
}