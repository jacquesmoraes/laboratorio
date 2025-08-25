
namespace Applications.Contracts.WebSiteServices
{
    public interface IWebsiteWorkTypeService
    {
        // For the homepage
        Task<IEnumerable<WebsiteWorkTypeDto>> GetActiveForWebsiteAsync();

        // For the admin panel
        Task<IEnumerable<WebsiteWorkTypeDto>> GetAllAsync();
        Task<WebsiteWorkTypeDto> GetByIdAsync(int id);
        
        // CRUD
        Task<WebsiteWorkTypeDto> CreateAsync(CreateWebsiteWorkTypeDto dto);
        Task<WebsiteWorkTypeDto> UpdateAsync(int id, UpdateWebsiteWorkTypeDto dto);

        // Status management
        Task ToggleActiveAsync (int id);

        // Ordering
        Task UpdateOrderAsync (List<ReorderItemDto> reorderItems);

        // Delete
        Task DeletePermanentlyAsync (int id);
    }
}