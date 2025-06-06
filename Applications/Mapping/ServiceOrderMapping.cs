using Applications.Dtos.ServiceOrder;
using Applications.Projections.ServiceOrder;
using AutoMapper;
using Core.Enums;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Applications.Records.ServiceOrders;

namespace Applications.Mapping
{
    public class ServiceOrderMappingProfile : Profile
    {
        public ServiceOrderMappingProfile()
        {
            // Entrada: criação de ordem e works
            CreateMap<CreateWorkDto, Work>();
            CreateMap<CreateServiceOrderDto, ServiceOrder>()
                .ForMember(dest => dest.Works, opt => opt.MapFrom(src => src.Works));

            // Resposta: detalhe completo
            CreateMap<ServiceOrder, ServiceOrderDetailsProjection>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CurrentSectorName, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.CurrentSectorName = ResolveCurrentSectorName(src);
                });

            // Resposta: lista
            CreateMap<ServiceOrder, ServiceOrderListProjection>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CurrentSectorName, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.CurrentSectorName = ResolveCurrentSectorName(src);
                });

            // Resposta: alerta
            CreateMap<ServiceOrder, ServiceOrderAlertRecord>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.ClientName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CurrentSectorName, opt => opt.Ignore())
                .ForMember(dest => dest.LastTryInDate, opt => opt.Ignore())
                .ForMember(dest => dest.DaysOut, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var lastStage = src.Stages
                        .Where(s => s.DateOut != null)
                        .OrderByDescending(s => s.DateOut)
                        .FirstOrDefault();

                    dest.CurrentSectorName = lastStage?.Sector?.Name ?? string.Empty;
                    dest.LastTryInDate = lastStage?.DateOut ?? DateTime.MinValue;
                    dest.DaysOut = lastStage?.DateOut is DateTime dt
                        ? (int)(DateTime.UtcNow - dt).TotalDays
                        : 0;
                });

            // Resposta: short para fatura, cliente, etc.
            CreateMap<ServiceOrder, ServiceOrderShortRecord>();
        }

        private static string? ResolveCurrentSectorName(ServiceOrder src)
        {
            var orderedStages = src.Stages.OrderByDescending(s => s.DateIn).ToList();

            if (!orderedStages.Any())
                return null;

            if (src.Status == OrderStatus.Production)
                return orderedStages.FirstOrDefault(s => s.DateOut == null)?.Sector?.Name;

            if (src.Status == OrderStatus.TryIn)
                return orderedStages
                    .Where(s => s.DateOut != null)
                    .OrderByDescending(s => s.DateOut)
                    .FirstOrDefault()
                    ?.Sector?.Name;

            return orderedStages
                .Where(s => s.DateOut != null)
                .OrderByDescending(s => s.DateOut)
                .FirstOrDefault()
                ?.Sector?.Name;
        }
    }
}
