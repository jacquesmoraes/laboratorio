namespace Applications.Contracts.WebSiteServices
{
    public interface IWebsiteCaseService
    {
        // For the homepage
        Task<IEnumerable<WebsiteCaseDto>> GetActiveForHomepageAsync ( );
        Task<WebsiteCaseDetailsDto> GetDetailsByIdAsync ( int id );

        // For the admin panel
        Task<IEnumerable<WebsiteCaseAdminDto>> GetAllAsync ( );
        Task<IEnumerable<WebsiteCaseAdminDto>> GetActiveAsync ( );
        Task<IEnumerable<WebsiteCaseAdminDto>> GetInactiveAsync ( );
        Task<WebsiteCaseDetailsDto> GetByIdAsync ( int id );

        // CRUD
        Task<WebsiteCaseDetailsDto> CreateAsync ( CreateWebsiteCaseDto dto );
        Task<WebsiteCaseDetailsDto> UpdateAsync ( int id, UpdateWebsiteCaseDto dto );

        // Status management
        Task ActivateCaseAsync ( int id );
        Task DeactivateCaseAsync ( int id );
        Task ToggleActiveAsync ( int id );

        // Ordering
        Task UpdateOrderAsync ( List<ReorderItemDto> reorderItems );
        Task DeleteCasePermanentlyAsync ( int id );
    }
}
