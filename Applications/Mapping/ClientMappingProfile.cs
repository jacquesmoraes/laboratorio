﻿using Applications.Records.Clients;
using Applications.Records.Payments;
using Applications.Projections.Clients;
using Applications.Dtos.Clients;
using Applications.Dtos.Payments;
using AutoMapper;
using Core.Models.Clients;
using Core.Models.Payments;

namespace Applications.Mapping
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            // Resposta curta (GET /clients)
            CreateMap<Client, ClientResponseRecord>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.TablePriceName, opt => opt.MapFrom(src => src.TablePrice!.Name));

            // Resposta detalhada (GET /clients/{id})
            CreateMap<Client, ClientResponseDetailsProjection>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
    .ForMember(dest => dest.BillingMode, opt => opt.MapFrom(src => (int)src.BillingMode)) 
    .ForMember(dest => dest.TablePriceName, opt => opt.MapFrom(src => src.TablePrice != null ? src.TablePrice.Name : null)); 

            

            // Projeção para tabela de preços
            CreateMap<Client, ClientResponseForTablePriceRecord>()
                .ForMember(dest => dest.TablePriceName, opt => opt.MapFrom(src => src.TablePrice != null ? src.TablePrice.Name : null));

            // Projeção de endereço
            CreateMap<Address, ClientAddressRecord>();
            CreateMap<Client, ClientInvoiceRecord>()
    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ClientPhoneNumber));


            // Projeção de pagamento
            CreateMap<Payment, ClientPaymentRecord>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.ClientName))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.BillingInvoice != null ? src.BillingInvoice.InvoiceNumber : null));

            // Criação de pagamento
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.PaymentDate, DateTimeKind.Utc)));

            // Criação de cliente
            CreateMap<CreateClientDto, Client>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ClientCpf, opt => opt.MapFrom(src => src.Cpf))
                .ForMember(dest => dest.ClientPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src =>
                    src.BirthDate.HasValue
                        ? DateTime.SpecifyKind(src.BirthDate.Value, DateTimeKind.Utc)
                        : (DateTime?)null));

            CreateMap<ClientAreaDataDto, ClientAreaRecord>()
    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.ClientName))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Client.ClientPhoneNumber))
    .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Client.Address.Street))
    .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Client.Address.Number))
    .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Client.Address.Complement))
    .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Client.Address.Neighborhood))
    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Client.Address.City))
    .ForMember(dest => dest.TotalPaid, opt => opt.MapFrom(src => src.TotalPaid))
    .ForMember(dest => dest.TotalInvoiced, opt => opt.MapFrom(src => src.TotalInvoiced))
    .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.Credit))
    .ForMember(dest => dest.Debit, opt => opt.MapFrom(src => src.Debit))
    .ForMember(dest => dest.Invoices, opt => opt.MapFrom(src => src.Invoices));

        }
    }
}
