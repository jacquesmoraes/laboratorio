namespace Applications.Mapping
{
    public class WorkMappingProfile : Profile
    {
        public WorkMappingProfile ( )
        {
            //entry via post/put
            CreateMap<CreateWorkTypeDto, WorkType> ( );
            CreateMap<UpdateWorkTypeDto, WorkType> ( );

            //works response for work type

            CreateMap<WorkType, WorkTypeResponseRecord> ( )
                .ForMember ( dest => dest.WorkSectionName, opt => opt.MapFrom ( src => src.WorkSection.Name ) );

            //works response for service order

            CreateMap<Work, WorkRecord> ( )
                .ForMember ( d => d.WorkTypeId, opt => opt.MapFrom ( s => s.WorkTypeId ) )
                .ForMember ( d => d.WorkTypeName, opt => opt.MapFrom ( s => s.WorkType.Name ) )
                .ForMember ( d => d.Quantity, opt => opt.MapFrom ( s => s.Quantity ) )
                .ForMember ( d => d.PriceUnit, opt => opt.MapFrom ( s => s.PriceUnit ) )
                .ForMember ( d => d.PriceTotal, opt => opt.MapFrom ( s => s.PriceTotal ) )
                .ForMember ( d => d.ShadeId, opt => opt.MapFrom ( s => s.ShadeId ) )         
                .ForMember ( d => d.ShadeColor, opt => opt.MapFrom ( s => s.Shade != null ? s.Shade.color : null ) )
                .ForMember ( d => d.ScaleId, opt => opt.MapFrom ( s => s.ScaleId ) )         
                .ForMember ( d => d.ScaleName, opt => opt.MapFrom ( s => s.Scale != null ? s.Scale.Name : null ) )
                .ForMember ( d => d.Notes, opt => opt.MapFrom ( s => s.Notes ) );




            // Applications/Mapping/WorkMappingProfile.cs
            CreateMap<WorkSection, WorkSectionRecord> ( );
            CreateMap<CreateWorkSectionDto, WorkSection> ( );
            CreateMap<UpdateWorkSectionDto, WorkSection> ( );

        }
    }
}
