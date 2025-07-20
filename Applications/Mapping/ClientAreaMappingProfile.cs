using Applications.Projections.ClientArea;

namespace Applications.Mapping
{
    public class ClientAreaMappingProfile : Profile
    {
        public ClientAreaMappingProfile ( )
        {
            CreateMap<BillingInvoice, ClientAreaInvoiceProjection> ( )
                .ForMember ( dest => dest.BillingInvoiceId,
                    opt => opt.MapFrom ( src => src.BillingInvoiceId ) )
                .ForMember ( dest => dest.InvoiceNumber,
                    opt => opt.MapFrom ( src => src.InvoiceNumber ) )
                .ForMember ( dest => dest.CreatedAt,
                    opt => opt.MapFrom ( src => src.CreatedAt ) )
                .ForMember ( dest => dest.Description,
                    opt => opt.MapFrom ( src => src.Description ) )
                .ForMember ( dest => dest.Status,
                    opt => opt.MapFrom ( src => src.Status ) )
                .ForMember ( dest => dest.TotalInvoiceAmount,
                    opt => opt.MapFrom ( src =>
                        src.TotalServiceOrdersAmount + src.PreviousDebit - src.PreviousCredit
                    ) );


            CreateMap<ServiceOrder, ClientAreaServiceOrderProjection>()
            .ForMember(dest => dest.ServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrderId))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
            .ForMember(dest => dest.DateIn, opt => opt.MapFrom(src => src.DateIn))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
            .ForMember(dest => dest.OrderTotal, opt => opt.MapFrom(src => src.OrderTotal))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
