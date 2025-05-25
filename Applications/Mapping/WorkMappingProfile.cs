using Applications.Dtos.ServiceOrder;
using Applications.Dtos.Work;
using AutoMapper;
using Core.Models.Works;

namespace Applications.Mapping
{
    public class WorkMappingProfile : Profile
    {

        public WorkMappingProfile ( )
        {
            CreateMap<CreateWorkTypeDto, WorkType> ( );
            CreateMap<UpdateWorkTypeDto, WorkType> ( );

            CreateMap<Work, WorkDto> ( )
    .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
    .ForMember ( dest => dest.ShadeColor, opt => opt.MapFrom ( src => src.Shade!.color ) )
    .ForMember ( dest => dest.ScaleName, opt => opt.MapFrom ( src => src.Shade!.Scale!.Name ) )
    .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<WorkType, WorkTypeResponseDto> ( )
                .ForMember ( dest => dest.WorkSectionName, opt => opt.MapFrom ( src => src.WorkSection.Name ) );

        }




    }
}
