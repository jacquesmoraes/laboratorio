namespace Applications.Mapping
{
    public class WorkMappingProfile : Profile
    {
        public WorkMappingProfile ( )
        {
            // Entrada via POST/PUT
            CreateMap<CreateWorkTypeDto, WorkType> ( );
            CreateMap<UpdateWorkTypeDto, WorkType> ( );

            // Resposta da listagem de tipos de trabalho
            CreateMap<WorkType, WorkTypeResponseRecord> ( )
                .ForMember ( dest => dest.WorkSectionName, opt => opt.MapFrom ( src => src.WorkSection.Name ) );

            // Resposta de trabalhos vinculados à ordem de serviço
            CreateMap<Work, WorkRecord> ( )
                .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
                .ForMember ( dest => dest.ShadeColor, opt => opt.MapFrom ( src => src.Shade!.color ) )
                .ForMember ( dest => dest.ScaleName, opt => opt.MapFrom ( src => src.Shade!.Scale!.Name ) )
                .ForMember ( dest => dest.Notes, opt => opt.MapFrom ( src => src.Notes ) );

            // Applications/Mapping/WorkMappingProfile.cs
            CreateMap<WorkSection, WorkSectionRecord> ( );

        }
    }
}
