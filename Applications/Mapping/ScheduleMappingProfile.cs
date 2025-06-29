namespace Applications.Mapping
{
    public class ScheduleMappingProfile : Profile
    {
        public ScheduleMappingProfile()
        {
            CreateMap<ScheduleDeliveryDto, ServiceOrderSchedule>();
            CreateMap<CreateScheduleDto, ServiceOrderSchedule>();

            CreateMap<ServiceOrderSchedule, ScheduleItemRecord>()
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceOrderId, opt => opt.MapFrom(src => src.ServiceOrderId))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.ServiceOrder.OrderNumber))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.ServiceOrder.PatientName))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ServiceOrder.Client.ClientName))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.ScheduledDate))
                .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => src.DeliveryType))
                .ForMember(dest => dest.IsDelivered, opt => opt.MapFrom(src => src.IsDelivered))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.IsDelivered ? "Delivered" :
                    src.IsOverdue ? "Overdue" :
                    src.ScheduledDate == DateTime.Today ? "Today" : "Scheduled"))
                .ForMember(dest => dest.CurrentSectorName, opt => opt.MapFrom<ScheduleCurrentSectorNameResolver>())
                .ForMember(dest => dest.TargetSectorName, opt => opt.MapFrom(src =>
                    src.Sector != null ? src.Sector.Name : null));
        }
    }
}
