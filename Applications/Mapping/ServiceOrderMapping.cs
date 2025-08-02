namespace Applications.Mapping
{
    public class ServiceOrderMappingProfile : Profile
    {
        public ServiceOrderMappingProfile ( )
        {
            // Entrada: criação de ordem e works
            CreateMap<CreateWorkDto, Work> ( );
            CreateMap<CreateServiceOrderDto, ServiceOrder> ( )
                .ForMember ( dest => dest.Works, opt => opt.MapFrom ( src => src.Works ) );


            CreateMap<ServiceOrder, InvoiceServiceOrderRecord> ( )
      .ForMember ( dest => dest.OrderCode, opt => opt.MapFrom ( src =>
          !string.IsNullOrWhiteSpace ( src.OrderNumber ) && src.ClientId > 0
              ? $"{src.OrderNumber}-{src.ClientId}"
              : "" ) )
      .ForMember ( dest => dest.FinishedAt, opt => opt.MapFrom ( src =>
          src.DateOutFinal ?? src.DateOut ) )
      .ForMember ( dest => dest.Subtotal, opt => opt.MapFrom ( src =>
          src.Works != null ? src.Works.Sum ( w => w.Quantity * w.PriceUnit ) : 0 ) );

            CreateMap<ServiceOrder, ServiceOrderListDto>()
    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.ClientName))
        .ForMember(dest => dest.OrderTotal, opt => opt.MapFrom(src => src.OrderTotal))
    .ForMember(dest => dest.LastMovement, opt => opt.MapFrom<LastMovementDateForListDtoResolver>())
    .ForMember(dest => dest.CurrentSectorName, opt => opt.MapFrom<CurrentSectorNameForListDtoResolver>());




            CreateMap<ServiceOrder, ServiceOrderDetailsProjection> ( )
    .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
     .ForMember ( dest => dest.Client, opt => opt.MapFrom ( src => src.Client ) )
     .ForMember ( dest => dest.OrderTotal, opt => opt.MapFrom ( src => src.OrderTotal ) )
    .ForMember ( dest => dest.CurrentSectorName, opt => opt.MapFrom<CurrentSectorNameResolver> ( ) );

            CreateMap<ServiceOrder, ServiceOrderListProjection> ( )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
                 .ForMember ( dest => dest.ClientId, opt => opt.MapFrom ( src => src.ClientId ) )
                 .ForMember ( dest => dest.LastMovementDate, opt => opt.MapFrom<LastMovementDateResolver> ( ) )
                .ForMember ( dest => dest.CurrentSectorName, opt => opt.MapFrom<CurrentSectorNameResolver> ( ) )
                .ForMember ( dest => dest.TotalAmount, opt => opt.MapFrom ( src => 
        src.Works != null ? src.Works.Sum ( w => w.Quantity * w.PriceUnit ) : 0 ) )
            .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) ); // Adicione esta linha


            CreateMap<ServiceOrder, ServiceOrderAlertRecord> ( )
                .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
                .ForMember ( dest => dest.CurrentSectorName, opt => opt.MapFrom<ServiceOrderAlertCurrentSectorNameResolver> ( ) )
                .ForMember ( dest => dest.LastTryInDate, opt => opt.MapFrom<ServiceOrderAlertLastTryInDateResolver> ( ) )
                .ForMember ( dest => dest.DaysOut, opt => opt.MapFrom<ServiceOrderAlertDaysOutResolver> ( ) );


                        
            CreateMap<ServiceOrder, ServiceOrderShortRecord> ( )
                .ForMember ( dest => dest.OrderNumber, opt => opt.MapFrom ( src => src.OrderNumber ) )
                .ForMember ( dest => dest.PatientName, opt => opt.MapFrom ( src => src.PatientName ) )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) );
            
        }


    }
}
