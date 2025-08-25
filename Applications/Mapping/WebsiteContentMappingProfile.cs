namespace Applications.Mapping
{
    public class WebsiteContentMappingProfile : Profile
    {
        public WebsiteContentMappingProfile()
        {
            // WebsiteCase mappings
            CreateMap<CreateWebsiteCaseDto, WebsiteCase>();
            CreateMap<UpdateWebsiteCaseDto, WebsiteCase>();
            CreateMap<WebsiteCase, WebsiteCaseDto>();
            CreateMap<WebsiteCase, WebsiteCaseDetailsDto>();
            CreateMap<WebsiteCase, WebsiteCaseAdminDto>()
                .ForMember(dest => dest.ImageCount, 
                          opt => opt.MapFrom(src => src.Images != null ? src.Images.Count : 0));
            
            // WebsiteCaseImage mappings
            CreateMap<CreateWebsiteCaseImageDto, WebsiteCaseImage>();
            CreateMap<WebsiteCaseImage, WebsiteCaseImageDto>();
            
            // WebsiteWorkType mappings
            CreateMap<CreateWebsiteWorkTypeDto, WebsiteWorkType>();
            CreateMap<UpdateWebsiteWorkTypeDto, WebsiteWorkType>();
            CreateMap<WebsiteWorkType, WebsiteWorkTypeDto>()
                .ForMember(dest => dest.WorkTypeName, 
                          opt => opt.MapFrom(src => src.WorkType!.Name))
                .ForMember(dest => dest.WorkTypeDescription, 
                          opt => opt.MapFrom(src => src.WorkType!.Description))
                .ForMember(dest => dest.WorkSectionName, 
                          opt => opt.MapFrom(src => src.WorkType!.WorkSection!.Name));
        }
    }
}