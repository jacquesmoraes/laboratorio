namespace Applications.Mapping
{
    public class ProductionMappingProfile : Profile
    {
        public ProductionMappingProfile ( )
        {
            // Criação (POST) — mantêm-se como class
            CreateMap<CreateScaleDto, Scale> ( );
            CreateMap<CreateShadeDto, Shade> ( );

            // Respostas (GET) — agora são records
            CreateMap<ProductionStage, StageRecord> ( )
                .ForMember ( dest => dest.SectorName, opt => opt.MapFrom ( src => src.Sector.Name ) );

            CreateMap<Scale, ScaleRecord> ( );
            CreateMap<Shade, ShadeRecord> ( );
        }
    }
}
