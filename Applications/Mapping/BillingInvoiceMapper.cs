using Applications.Dtos.Billing;
using Applications.Dtos.Clients;
using AutoMapper;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.ServiceOrders;
using Core.Models.Works;

namespace Applications.Mappers
{
    public class BillingInvoiceProfile : Profile
    {
        public BillingInvoiceProfile()
        {
            CreateMap<BillingInvoice, BillingInvoiceResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Client, ClientResponseForOrderServiceDto>();

            CreateMap<ServiceOrder, InvoiceServiceOrderDto>()
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => src.DateIn))
                .ForMember(dest => dest.FinishedAt, opt => opt.MapFrom(src => src.DateOut))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.OrderTotal))
                .ForMember(dest => dest.OrderNumberFormatted, opt => opt.MapFrom(src =>
                    $"{src.OrderNumber:D5}-{src.ServiceOrderId}"));

            CreateMap<Work, InvoiceWorkItemDto>()
                .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom(src => src.WorkType.Name));
             }
    }
}
