namespace Applications.Mapping
{
    public class BillingInvoiceProfile : Profile
    {
        public BillingInvoiceProfile ( )
        {
            // Projeção para tela (resumo de fatura)
            CreateMap<BillingInvoice, BillingInvoiceResponseProjection> ( )
                .ForMember ( dest => dest.TotalInvoiceAmount,
                    opt => opt.MapFrom ( src => src.TotalServiceOrdersAmount + src.PreviousDebit - src.PreviousCredit ) )
                .ForMember ( dest => dest.TotalPaid,
                    opt => opt.MapFrom ( src => src.Payments != null ? src.Payments.Sum ( p => p.AmountPaid ) : 0m ) );

            // Projeção principal para geração de PDF
            CreateMap<BillingInvoice, BillingInvoiceRecord> ( )
                .ForMember ( dest => dest.ClientId, opt => opt.MapFrom ( src => src.ClientId ) );

            CreateMap<Client, ClientInvoiceRecord> ( )
                .ForMember ( dest => dest.Address,
                    opt => opt.MapFrom ( src => $"{src.Address.Street}, {src.Address.Number}" ) )
                .ForMember ( dest => dest.PhoneNumber,
                    opt => opt.MapFrom ( src => src.ClientPhoneNumber ) );

            CreateMap<ServiceOrder, InvoiceServiceOrderRecord> ( )
                .ForMember ( dest => dest.OrderCode, opt => opt.MapFrom ( src => $"{src.OrderNumber}-1" ) )
                .ForMember ( dest => dest.FinishedAt, opt => opt.MapFrom ( src => src.DateOutFinal ?? src.DateOut ) );

            CreateMap<Work, InvoiceWorkItemRecord> ( )
                .ForMember ( dest => dest.WorkTypeName, opt => opt.MapFrom ( src => src.WorkType.Name ) );
            
        }
    }
}
