using Applications.Dtos.Pricing;
using Applications.Mapping.Resolvers;
using AutoMapper;
using Core.Models.Pricing;

namespace Applications.Mapping
{
    public class PricingMappingProfile : Profile
    {
        public PricingMappingProfile ( )
        {
            CreateMap<CreateTablePriceItemDto, TablePriceItem> ( )
                .ForMember ( dest => dest.TablePrice, opt => opt.Ignore ( ) )
                .ForMember ( dest => dest.WorkType, opt => opt.Ignore ( ) );
            CreateMap<TablePrice, TablePriceResponseDto> ( ).ReverseMap ( );

            CreateMap<CreateTablePriceDto, TablePrice> ( );
           
            CreateMap<TablePriceItem, CreateTablePriceItemDto> ( );
          CreateMap<TablePriceItem, TablePriceItemsResponseDto>()
    .ForMember(dest => dest.TablePriceName, opt => opt.MapFrom<TablePriceNameResolver>())
    .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom<WorkTypeNameForResponseDtoResolver>())
    .ReverseMap();

CreateMap<TablePriceItem, TablePriceItemShortDto>()
    .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom<WorkTypeNameResolver>());




            CreateMap<UpdateTablePriceDto, TablePrice> ( );
            CreateMap<UpdateTablePriceItemDto, TablePriceItem> ( );

            


        }
    }

}
