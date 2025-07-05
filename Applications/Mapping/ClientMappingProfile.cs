namespace Applications.Mapping
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile ( )
        {
            // Short response (GET /clients)
            CreateMap<Client, ClientResponseRecord> ( )
                .ForMember ( dest => dest.City, opt => opt.MapFrom ( src => src.Address.City ) )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice!.Name ) );

            // Detailed response (GET /clients/{id})
            // ... existing code ...
            // Detailed response (GET /clients/{id})
            CreateMap<Client, ClientResponseDetailsProjection> ( )
                .ForMember ( dest => dest.Address, opt => opt.MapFrom ( src => src.Address ) )
                .ForMember ( dest => dest.City, opt => opt.MapFrom ( src => src.Address.City ) )
                .ForMember ( dest => dest.BillingMode, opt => opt.MapFrom ( src => ( int ) src.BillingMode ) )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) )
                .ForMember ( dest => dest.TotalPaid, opt => opt.Ignore ( ) )
                .ForMember ( dest => dest.TotalInvoiced, opt => opt.Ignore ( ) )
                .ForMember ( dest => dest.ServiceOrders, opt => opt.MapFrom ( src => src.ServiceOrders ) )
                .ForMember ( dest => dest.Payments, opt => opt.MapFrom ( src => src.Payments ) );
            // ... existing code ...

            // Projection for price table
            CreateMap<Client, ClientResponseForTablePriceRecord> ( )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) );

            // Address projection
            CreateMap<Address, ClientAddressRecord> ( );
            CreateMap<ClientAddressRecord, Address> ( );

            // Projection for invoice
            CreateMap<Client, ClientInvoiceRecord> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.ClientName ) )
                .ForMember ( dest => dest.Address, opt => opt.MapFrom ( src => src.Address ) )
                .ForMember ( dest => dest.PhoneNumber, opt => opt.MapFrom ( src => src.ClientPhoneNumber ) );

            // Projection for payment
            CreateMap<Payment, ClientPaymentRecord> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) )
                .ForMember ( dest => dest.InvoiceNumber, opt => opt.MapFrom ( src => src.BillingInvoice != null ? src.BillingInvoice.InvoiceNumber : null ) );

            // Payment creation
            CreateMap<CreatePaymentDto, Payment> ( )
                .ForMember ( dest => dest.PaymentDate, opt => opt.MapFrom ( src => DateTime.SpecifyKind ( src.PaymentDate, DateTimeKind.Utc ) ) );

            // Client creation
            CreateMap<CreateClientDto, Client> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Name ) )
                .ForMember ( dest => dest.ClientEmail, opt => opt.MapFrom ( src => src.Email ) )
                .ForMember ( dest => dest.ClientCpf, opt => opt.MapFrom ( src => src.Cpf ) )
                .ForMember ( dest => dest.ClientPhoneNumber, opt => opt.MapFrom ( src => src.PhoneNumber ) )
                .ForMember ( dest => dest.BirthDate, opt => opt.MapFrom ( src =>
                    src.BirthDate.HasValue
                        ? DateTime.SpecifyKind ( src.BirthDate.Value, DateTimeKind.Utc )
                        : ( DateTime? ) null ) )
                .ForMember ( dest => dest.Address, opt => opt.MapFrom ( src => src.Address ) );
        }
    }
}
