
namespace Applications.Mapping
{
    public class PricingMappingProfile : Profile
    {
        public PricingMappingProfile ( )
        {
            // Criação (POST)
            CreateMap<CreateTablePriceDto, TablePrice> ( )
                .ForMember ( dest => dest.Items, opt => opt.Ignore ( ) ); // vai ser resolvido manualmente no serviço

            CreateMap<CreateTablePriceItemDto, TablePriceItem> ( )
                .ForMember ( dest => dest.TablePrice, opt => opt.Ignore ( ) );

            // Atualização (PUT)
            CreateMap<UpdateTablePriceDto, TablePrice> ( );
            CreateMap<UpdateTablePriceItemDto, TablePriceItem> ( )
                .ForMember ( dest => dest.TablePrice, opt => opt.Ignore ( ) );

            // Respostas resumidas de item
            CreateMap<TablePriceItem, TablePriceItemShortRecord> ( )
                .ForMember ( dest => dest.Id, opt => opt.MapFrom ( src => src.TablePriceItemId ) )
                .ForMember ( dest => dest.ItemName, opt => opt.MapFrom ( src => src.ItemName ) )
                .ForMember ( dest => dest.Price, opt => opt.MapFrom ( src => src.Price ) );

            // Resposta detalhada de item
            CreateMap<TablePriceItem, TablePriceItemsResponseRecord> ( )
                .ForMember ( dest => dest.Id, opt => opt.MapFrom ( src => src.TablePriceItemId ) )
                .ForMember ( dest => dest.ItemName, opt => opt.MapFrom ( src => src.ItemName ) )
                .ForMember ( dest => dest.Price, opt => opt.MapFrom ( src => src.Price ) )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) );

            // Resposta completa da tabela de preços
            CreateMap<TablePrice, TablePriceResponseProjection> ( )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status ) );
        }
    }
}