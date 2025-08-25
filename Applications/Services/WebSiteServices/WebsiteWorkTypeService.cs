namespace Applications.Services.WebSiteServices
{
    public class WebsiteWorkTypeService : IWebsiteWorkTypeService
    {
        private readonly IGenericRepository<WebsiteWorkType> _websiteWorkTypeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WebsiteWorkTypeService (
            IGenericRepository<WebsiteWorkType> websiteWorkTypeRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper )
        {
            _websiteWorkTypeRepo = websiteWorkTypeRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WebsiteWorkTypeDto>> GetActiveForWebsiteAsync ( )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ActiveWithWorkType();
            var workTypes = await _websiteWorkTypeRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteWorkTypeDto>> ( workTypes );
        }

        public async Task<IEnumerable<WebsiteWorkTypeDto>> GetAllAsync ( )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.AllWithWorkType();
            var workTypes = await _websiteWorkTypeRepo.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<WebsiteWorkTypeDto>> ( workTypes );
        }

        public async Task<WebsiteWorkTypeDto> GetByIdAsync ( int id )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ByIdWithWorkType(id);
            var workType = await _websiteWorkTypeRepo.GetEntityWithSpec(spec);

            if ( workType == null )
                throw new NotFoundException ( "Website work type not found." );

            return _mapper.Map<WebsiteWorkTypeDto> ( workType );
        }

        public async Task<WebsiteWorkTypeDto> CreateAsync ( CreateWebsiteWorkTypeDto dto )
        {
            var workType = _mapper.Map<WebsiteWorkType>(dto);
            workType.CreatedAt = DateTime.UtcNow;
            workType.UpdatedAt = DateTime.UtcNow;

            var created = await _websiteWorkTypeRepo.CreateAsync(workType);
            await _unitOfWork.SaveChangesAsync ( );

            return await GetByIdAsync ( created.Id );
        }

        public async Task<WebsiteWorkTypeDto> UpdateAsync ( int id, UpdateWebsiteWorkTypeDto dto )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ByIdWithWorkType(id);
            var workType = await _websiteWorkTypeRepo.GetEntityWithSpec(spec);

            if ( workType == null )
                throw new NotFoundException ( "Website work type not found." );

            _mapper.Map ( dto, workType );
            workType.UpdatedAt = DateTime.UtcNow;

            await _websiteWorkTypeRepo.UpdateAsync ( id, workType );
            await _unitOfWork.SaveChangesAsync ( );

            return await GetByIdAsync ( id );
        }

        public async Task ToggleActiveAsync ( int id )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ByIdWithWorkType(id);
            var workType = await _websiteWorkTypeRepo.GetEntityWithSpec(spec);
            if ( workType == null )
                throw new NotFoundException ( "Website work type not found." );

            workType.IsActive = !workType.IsActive;
            workType.UpdatedAt = DateTime.UtcNow;

            await _websiteWorkTypeRepo.UpdateAsync ( id, workType );
            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task UpdateOrderAsync ( List<ReorderItemDto> reorderItems )
        {
            foreach ( var item in reorderItems )
            {
                var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ByIdWithWorkType(item.Id);
                var workType = await _websiteWorkTypeRepo.GetEntityWithSpec(spec);
                if ( workType != null )
                {
                    workType.Order = item.Order;
                    workType.UpdatedAt = DateTime.UtcNow;
                    await _websiteWorkTypeRepo.UpdateAsync ( item.Id, workType );
                }
            }

            await _unitOfWork.SaveChangesAsync ( );
        }

        public async Task DeletePermanentlyAsync ( int id )
        {
            var spec = WebsiteWorkTypeSpecification.WebsiteWorkTypeSpecs.ByIdWithWorkType(id);
            var workType = await _websiteWorkTypeRepo.GetEntityWithSpec(spec);
            if ( workType == null )
                throw new NotFoundException ( "Website work type not found." );

            await _websiteWorkTypeRepo.DeleteAsync ( workType );
            await _unitOfWork.SaveChangesAsync ( );
        }
    }
}