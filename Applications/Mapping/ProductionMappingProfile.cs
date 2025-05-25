using Applications.Dtos.Production;
using AutoMapper;
using Core.Models.Production;
using Core.Models.ServiceOrders;

namespace Applications.Mapping
{
    public class ProductionMappingProfile : Profile
    {
        public ProductionMappingProfile ( )
        {


            CreateMap<ProductionStage, StageDto> ( )
            .ForMember ( dest => dest.SectorName, opt => opt.MapFrom ( src => src.Sector.Name ) );
            CreateMap<Scale, ScaleDto> ( );
            CreateMap<CreateScaleDto, Scale> ( );

            CreateMap<Shade, ShadeDto> ( );
            CreateMap<CreateShadeDto, Shade> ( );

        }

    }
}
