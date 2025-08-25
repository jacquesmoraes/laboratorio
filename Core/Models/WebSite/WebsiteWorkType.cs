using Core.Models.Works;

namespace Core.Models.WebSite
{
    public class WebsiteWorkType
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public WorkType? WorkType { get; set; }
    }
}