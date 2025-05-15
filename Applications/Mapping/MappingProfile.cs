using Applications.Dtos.Clients;
using Applications.Dtos.Pricing;
using Applications.Dtos.Work;
using AutoMapper;
using Core.Models.Clients;
using Core.Models.Pricing;
using Core.Models.ServiceOrders;
using Core.Models.Works;

namespace Applications.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile ( )
        {
            CreateMap<CreateTablePriceItemDto, TablePriceItem> ( )
                .ForMember ( dest => dest.TablePrice, opt => opt.Ignore ( ) )
                .ForMember ( dest => dest.WorkType, opt => opt.Ignore ( ) );
            CreateMap<TablePriceItem, CreateTablePriceItemDto> ( );
            CreateMap<TablePriceItem, TablePriceItemsResponseDto> ( )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice.Name ) )
                .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) );

            CreateMap<TablePriceItem, TablePriceItemsResponseDto> ( ).ReverseMap ( );
            CreateMap<CreateWorkTypeDto, WorkType> ( );
            CreateMap<UpdateWorkTypeDto, WorkType> ( );

            CreateMap<WorkType, WorkTypeResponseDto> ( )
                .ForMember ( dest => dest.WorkSectionName, opt => opt.MapFrom ( src => src.WorkSection.Name ) );
            CreateMap<Client, ClientResponseDto> ( )
    .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) )
    .ForMember ( dest => dest.Street, opt => opt.MapFrom ( src => src.Address.Street ) )
    .ForMember ( dest => dest.Number, opt => opt.MapFrom ( src => src.Address.Number ) )
    .ForMember ( dest => dest.Complement, opt => opt.MapFrom ( src => src.Address.Complement ) )
    .ForMember ( dest => dest.Neighborhood, opt => opt.MapFrom ( src => src.Address.Neighborhood ) )
    .ForMember ( dest => dest.City, opt => opt.MapFrom ( src => src.Address.City ) )
    .ForMember ( dest => dest.IsInactive, opt => opt.MapFrom ( src => src.IsInactive ) );

            CreateMap<Patient, PatientDto> ( );
            CreateMap<ClientPayment, ClientPaymentDto> ( );
            CreateMap<ServiceOrder, ServiceOrderDto> ( );

        }

    }
}
