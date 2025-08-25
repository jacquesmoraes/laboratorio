namespace Applications.Dtos.WebSite
{
    public class UpdateWebsiteWorkTypeDto
    {
        public int WorkTypeId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}
