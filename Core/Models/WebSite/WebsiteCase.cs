namespace Core.Models.WebSite
{
    public class WebsiteCase
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public string Materials { get; set; } = string.Empty;
        public string Procedure { get; set; } = string.Empty;
        public string Results { get; set; } = string.Empty;
        public string PatientInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        
        public ICollection<WebsiteCaseImage> Images { get; set; } = [];
    }
}