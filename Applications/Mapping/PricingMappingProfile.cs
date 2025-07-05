namespace Applications.Mapping
{
    public class PricingMappingProfile : Profile
    {
        public PricingMappingProfile ( )
        {
            // Criação (POST)
            CreateMap<CreateTablePriceDto, TablePrice> ( )
                .ForMember ( dest => dest.Items, opt => opt.Ignore ( ) );



            // Record de item individual usado em preenchimento automático (lookup)
            CreateMap<TablePriceItem, TablePriceItemRecord> ( )
                .ForMember ( dest => dest.WorkTypeId, opt => opt.MapFrom ( src => src.WorkTypeId ) )
                .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
                .ForMember ( dest => dest.Price, opt => opt.MapFrom ( src => src.Price ) );

            // Resposta completa de um item isolado
            CreateMap<TablePriceItem, TablePriceItemResponseRecord> ( )
                .ForMember ( dest => dest.Id, opt => opt.MapFrom ( src => src.TablePriceItemId ) )
                .ForMember ( dest => dest.WorkTypeId, opt => opt.MapFrom ( src => src.WorkTypeId ) )
                .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
                .ForMember ( dest => dest.Price, opt => opt.MapFrom ( src => src.Price ) )
                .ForMember ( dest => dest.TablePriceId, opt => opt.MapFrom ( src => src.TablePriceId ) )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : string.Empty ) );

            // Resposta projetada de uma tabela de preços com clientes
            CreateMap<TablePrice, TablePriceResponseProjection> ( )
                .ForMember ( dest => dest.Items, opt => opt.MapFrom ( src => src.Items ) )
                .ForMember ( dest => dest.Clients, opt => opt.MapFrom ( src => src.Clients ) );

            // Mapeamento de Client → ClientResponseForTablePriceRecord
            CreateMap<Core.Models.Clients.Client, ClientResponseForTablePriceRecord> ( )
                .ForMember ( dest => dest.ClientId, opt => opt.MapFrom ( src => src.ClientId ) )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.ClientName ) )
                .ForMember ( dest => dest.BillingMode, opt => opt.MapFrom ( src => ( int ) src.BillingMode ) )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : string.Empty ) );
        }
    }
}
