namespace Core.Models.WebSite
{
    public class WebsiteCaseImage
    {
        public int Id { get; set; }
        public int WebsiteCaseId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsMainImage { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public  WebsiteCase? WebsiteCase { get; set; }
    }
}