using Applications.Dtos.Pricing;
using Applications.Records.Pricing;
using Applications.Projections.Pricing;
using Applications.Mapping.Resolvers;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping
{
    public class PricingMappingProfile : Profile
    {
        public PricingMappingProfile()
        {
            // Criação de itens e tabela de preços (entrada POST)
            CreateMap<CreateTablePriceDto, TablePrice>();
            CreateMap<CreateTablePriceItemDto, TablePriceItem>()
                .ForMember(dest => dest.TablePrice, opt => opt.Ignore())
                .ForMember(dest => dest.WorkType, opt => opt.Ignore());

            // Atualização de tabela de preços (entrada PUT)
            CreateMap<UpdateTablePriceDto, TablePrice>();
            CreateMap<UpdateTablePriceItemDto, TablePriceItem>();

            // Resposta resumida de item
            CreateMap<TablePriceItem, TablePriceItemShortRecord>()
                .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom<WorkTypeNameForShortRecordResolver>());

            // Resposta detalhada de item
            CreateMap<TablePriceItem, TablePriceItemsResponseRecord>()
                .ForMember(dest => dest.TablePriceName, opt => opt.MapFrom<TablePriceNameResolver>())
                .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom<WorkTypeNameForResponseRecordResolver>());

            // Resposta completa da tabela de preços
            CreateMap<TablePrice, TablePriceResponseProjection>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
