using Applications.Dtos.Pricing;
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
            CreateMap<CreateTablePriceItemDto, TablePriceItem> ( );
            CreateMap<TablePriceItem, CreateTablePriceItemDto> ( );
            CreateMap<TablePriceItem, TablePriceItemsResponseDto> ( )
     .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice.Name ) )
     .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) )
     .ReverseMap ( );
            CreateMap<TablePriceItem, TablePriceItemShortDto> ( )
    .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) );

            CreateMap<UpdateTablePriceDto, TablePrice> ( );
            CreateMap<UpdateTablePriceItemDto, TablePriceItem> ( );

            CreateMap<TablePriceItemShortDto, TablePriceItem> ( );


        }
    }

}
