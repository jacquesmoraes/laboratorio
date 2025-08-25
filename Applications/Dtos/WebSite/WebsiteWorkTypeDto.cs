namespace Applications.Dtos.WebSite
{
    public class WebsiteWorkTypeDto
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; }
        public string WorkTypeName { get; set; } = string.Empty;
        public string WorkTypeDescription { get; set; } = string.Empty;
        public string WorkSectionName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
