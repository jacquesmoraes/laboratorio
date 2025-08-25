using Applications.Contracts.WebSiteServices;
using Core.FactorySpecifications.WebSiteSpecifications;
using Core.Models.WebSite;

namespace Applications.Services.WebSiteServices
{
    public class WebsiteCaseService : IWebsiteCaseService
    {
        private readonly IGenericRepository<WebsiteCase> _websiteCaseRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WebsiteCaseService (
            IGenericRepository<WebsiteCase> websiteCaseRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper )
        {
            _websiteCaseRepo = websiteCaseRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WebsiteCaseDto>> GetActiveForHomepageAsync ( )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ActiveForHomepage();
            var cases = await _websiteCaseRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteCaseDto>> ( cases );
        }

        public async Task<WebsiteCaseDetailsDto> GetDetailsByIdAsync ( int id )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);

            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            return _mapper.Map<WebsiteCaseDetailsDto> ( caseEntity );
        }

        public async Task<IEnumerable<WebsiteCaseAdminDto>> GetAllAsync ( )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.AllWithImages();
            var cases = await _websiteCaseRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteCaseAdminDto>> ( cases );
        }

        public async Task<IEnumerable<WebsiteCaseAdminDto>> GetActiveAsync ( )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ActiveWithImages();
            var cases = await _websiteCaseRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteCaseAdminDto>> ( cases );
        }

        public async Task<IEnumerable<WebsiteCaseAdminDto>> GetInactiveAsync ( )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.InactiveWithImages();
            var cases = await _websiteCaseRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteCaseAdminDto>> ( cases );
        }

        public async Task<WebsiteCaseDetailsDto> GetByIdAsync ( int id )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);

            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            return _mapper.Map<WebsiteCaseDetailsDto> ( caseEntity );
        }

        public async Task<WebsiteCaseDetailsDto> CreateAsync ( CreateWebsiteCaseDto dto )
        {
            var caseEntity = _mapper.Map<WebsiteCase>(dto);
            caseEntity.CreatedAt = DateTime.UtcNow;
            caseEntity.UpdatedAt = DateTime.UtcNow;

            var created = await _websiteCaseRepo.CreateAsync(caseEntity);
            await _unitOfWork.SaveChangesAsync ( );

            return await GetByIdAsync ( created.Id );
        }

        public async Task<WebsiteCaseDetailsDto> UpdateAsync ( int id, UpdateWebsiteCaseDto dto )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);

            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            _mapper.Map ( dto, caseEntity );
            caseEntity.UpdatedAt = DateTime.UtcNow;

            await _websiteCaseRepo.UpdateAsync ( id, caseEntity );
            await _unitOfWork.SaveChangesAsync ( );

            return await GetByIdAsync ( id );
        }

        public async Task ActivateCaseAsync ( int id )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);
            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            caseEntity.IsActive = true;
            caseEntity.UpdatedAt = DateTime.UtcNow;

            await _websiteCaseRepo.UpdateAsync ( id, caseEntity );
            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task DeactivateCaseAsync ( int id )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);
            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            caseEntity.IsActive = false;
            caseEntity.UpdatedAt = DateTime.UtcNow;

            await _websiteCaseRepo.UpdateAsync ( id, caseEntity );
            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task ToggleActiveAsync ( int id )
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);
            if ( caseEntity == null )
                throw new NotFoundException ( "Case not found." );

            caseEntity.IsActive = !caseEntity.IsActive;
            caseEntity.UpdatedAt = DateTime.UtcNow;

            await _websiteCaseRepo.UpdateAsync ( id, caseEntity );
            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task UpdateOrderAsync ( List<ReorderItemDto> reorderItems )
        {
            foreach ( var item in reorderItems )
            {
                var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(item.Id);
                var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);
                if ( caseEntity != null )
                {
                    caseEntity.Order = item.Order;
                    caseEntity.UpdatedAt = DateTime.UtcNow;
                    await _websiteCaseRepo.UpdateAsync ( item.Id, caseEntity );
                }
            }

            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task DeleteCasePermanentlyAsync(int id)
        {
            var spec = WebsiteCaseSpecification.WebsiteCaseSpecs.ByIdWithImages(id);
            var caseEntity = await _websiteCaseRepo.GetEntityWithSpec(spec);

            if (caseEntity == null)
                throw new NotFoundException("Case not found.");

            await _websiteCaseRepo.DeleteAsync(caseEntity);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
