namespace Applications.Contracts.WebSiteServices
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(FileUploadModel file);
        Task<bool> DeleteImageAsync(string imageUrl);
        Task<bool> ImageExistsAsync(string imageUrl);
    }
}