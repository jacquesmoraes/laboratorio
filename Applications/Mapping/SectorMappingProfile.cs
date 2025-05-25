using Applications.Dtos.Sector;
using AutoMapper;
using Core.Models.ServiceOrders;

namespace Applications.Mapping
{
    public class SectorMappingProfile : Profile
    {
        public SectorMappingProfile ( )
        {
            CreateMap<Sector, SectorDto> ( );
            CreateMap<CreateSectorDto, Sector> ( );
            CreateMap<UpdateSectorDto, Sector> ( );
        }
    }
}