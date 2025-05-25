using Applications.Dtos.Clients;
using Applications.Dtos.Payments;
using AutoMapper;
using Core.Models.Clients;

namespace Applications.Mapping
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile ( )
        {
            CreateMap<Client, ClientResponseDto> ( )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) )
                .ForMember ( dest => dest.Street, opt => opt.MapFrom ( src => src.Address.Street ) )
                .ForMember ( dest => dest.Number, opt => opt.MapFrom ( src => src.Address.Number ) )
                .ForMember ( dest => dest.Complement, opt => opt.MapFrom ( src => src.Address.Complement ) )
                .ForMember ( dest => dest.Neighborhood, opt => opt.MapFrom ( src => src.Address.Neighborhood ) )
                .ForMember ( dest => dest.City, opt => opt.MapFrom ( src => src.Address.City ) )
                .ForMember ( dest => dest.IsInactive, opt => opt.MapFrom ( src => src.IsInactive ) );

            CreateMap<Patient, PatientDto> ( );
            CreateMap<PerClientPayment, ClientPaymentDto> ( )
                .ForMember ( dest => dest.Amount, opt => opt.MapFrom ( src => src.AmountPaid ) );
            CreateMap<CreatePerClientPaymentDto, PerClientPayment> ( );
            CreateMap<CreatePerClientPaymentDto, PerClientPayment> ( )
                .ForMember ( dest => dest.PaymentDate, opt => opt.MapFrom ( src => DateTime.SpecifyKind ( src.PaymentDate, DateTimeKind.Utc ) ) );
            CreateMap<PerClientPayment, PerClientPaymentDto> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) );
            CreateMap<Client, ClientResponseForTablePriceDto> ( )
                .ForMember ( dest => dest.TablePriceName, opt => opt.MapFrom ( src => src.TablePrice != null ? src.TablePrice.Name : null ) );
            CreateMap<Client, ClientResponseForOrderServiceDto> ( );
            CreateMap<CreateClientDto, Client> ( )
    .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Name ) )
    .ForMember ( dest => dest.ClientEmail, opt => opt.MapFrom ( src => src.Email ) )
    .ForMember ( dest => dest.ClientCpf, opt => opt.MapFrom ( src => src.Cpf ) )
    .ForMember ( dest => dest.ClientPhoneNumber, opt => opt.MapFrom ( src => src.PhoneNumber ) )
    .ForMember ( dest => dest.BirthDate, opt => opt.MapFrom ( src =>
        src.BirthDate.HasValue
            ? DateTime.SpecifyKind ( src.BirthDate.Value, DateTimeKind.Utc )
            : (DateTime?)null) );
            CreateMap<AddressDto, Address> ( );



        }
    }

}
