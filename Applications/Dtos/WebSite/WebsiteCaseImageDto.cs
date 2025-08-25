namespace Applications.Dtos.WebSite
{
    public class WebsiteCaseImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsMainImage { get; set; }
    }
}
