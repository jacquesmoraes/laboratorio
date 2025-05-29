using Applications.Dtos.Payments;
using AutoMapper;
using Core.Models.Payments;

namespace Applications.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile ( )
        {

            CreateMap<CreatePaymentDto, Payment> ( )
                .ForMember ( dest => dest.PaymentDate, opt => opt.MapFrom ( src => DateTime.SpecifyKind ( src.PaymentDate, DateTimeKind.Utc ) ) );



            CreateMap<Payment, ClientPaymentDto> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) )
                .ForMember ( dest => dest.InvoiceNumber, opt => opt.MapFrom ( src => src.BillingInvoice != null ? src.BillingInvoice.InvoiceNumber : null ) );
        }
    }

}
