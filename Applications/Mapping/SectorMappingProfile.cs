using Applications.Records.Sector;
using Applications.Dtos.Sector;
using AutoMapper;
using Core.Models.ServiceOrders;

namespace Applications.Mapping
{
    public class SectorMappingProfile : Profile
    {
        public SectorMappingProfile()
        {
            // Resposta GET
            CreateMap<Sector, SectorRecord>();

            // Entradas POST/PUT
            CreateMap<CreateSectorDto, Sector>();
            CreateMap<UpdateSectorDto, Sector>();
        }
    }
}
