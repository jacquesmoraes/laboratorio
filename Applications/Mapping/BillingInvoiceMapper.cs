using Applications.Records.Billing;
using Applications.Records.Clients;
using Applications.Projections.Billing;
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
            // Projeção principal da fatura
            CreateMap<BillingInvoice, BillingInvoiceResponseProjection>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TotalServiceOrdersAmount, opt => opt.MapFrom(src => src.TotalServiceOrdersAmount))
                .ForMember(dest => dest.PreviousCredit, opt => opt.MapFrom(src => src.PreviousCredit))
                .ForMember(dest => dest.PreviousDebit, opt => opt.MapFrom(src => src.PreviousDebit))
                .ForMember(dest => dest.TotalInvoiceAmount, opt => opt.MapFrom(src =>
                    src.TotalServiceOrdersAmount + src.PreviousDebit - src.PreviousCredit))
                .ForMember(dest => dest.TotalPaid, opt => opt.MapFrom(src =>
                    src.Payments != null ? src.Payments.Sum(p => p.AmountPaid) : 0m));

            // Projeção resumida do cliente na fatura
            CreateMap<Client, ClientInvoiceRecord>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address.Street + ", " + src.Address.Number))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ClientPhoneNumber));

            // Projeção de OS incluída na fatura
            CreateMap<ServiceOrder, InvoiceServiceOrderRecord>()
                .ForMember(dest => dest.OrderCode, opt => opt.MapFrom(src => $"{src.OrderNumber}-1"))
                .ForMember(dest => dest.Works, opt => opt.MapFrom(src => src.Works))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.OrderTotal))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.DateIn, opt => opt.MapFrom(src => src.DateIn))
                .ForMember(dest => dest.FinishedAt, opt => opt.MapFrom(src => src.DateOutFinal ?? src.DateOut));

            // Projeção de item de trabalho na fatura
            CreateMap<Work, InvoiceWorkItemRecord>()
                .ForMember(dest => dest.WorkTypeName, opt => opt.MapFrom(src => src.WorkType.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.PriceUnit, opt => opt.MapFrom(src => src.PriceUnit));
        }
    }
}
