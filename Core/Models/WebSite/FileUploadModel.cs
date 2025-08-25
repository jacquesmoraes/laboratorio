namespace Core.Models.WebSite
{
    // file upload model here because the layer architecture not allow us to access iformfile in application
   public class FileUploadModel
    {
        public Stream Content { get; set; } = default!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public long FileSize { get; set; }
    }
}
