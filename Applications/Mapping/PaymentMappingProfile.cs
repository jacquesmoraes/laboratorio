﻿namespace Applications.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile ( )
        {
            // Mapeamento de criação — entrada (mantém como está)
            CreateMap<CreatePaymentDto, Payment> ( )
                .ForMember ( dest => dest.PaymentDate, opt => opt.MapFrom ( src => DateTime.SpecifyKind ( src.PaymentDate, DateTimeKind.Utc ) ) );

            // Mapeamento de retorno — substitui ClientPaymentDto por record
            CreateMap<Payment, ClientPaymentRecord> ( )
    .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src =>
        src.Client != null ? src.Client.ClientName : string.Empty ) ) 
    .ForMember ( dest => dest.InvoiceNumber, opt => opt.MapFrom ( src =>
        src.BillingInvoice != null ? src.BillingInvoice.InvoiceNumber : null ) );
        }
    }
}
