namespace Applications.Dtos.WebSite
{
    public class WebsiteCaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}
