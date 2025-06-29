namespace Applications.Mapping
{
    public class SectorMappingProfile : Profile
    {
        public SectorMappingProfile ( )
        {
            // Resposta GET
            CreateMap<Sector, SectorRecord> ( );

            // Entradas POST/PUT
            CreateMap<CreateSectorDto, Sector> ( );
            CreateMap<UpdateSectorDto, Sector> ( );
        }
    }
}
