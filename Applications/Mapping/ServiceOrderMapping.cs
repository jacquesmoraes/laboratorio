using Applications.Dtos.ServiceOrder;
using AutoMapper;
using Core.Enums;
using Core.Models.ServiceOrders;
using Core.Models.Works;

namespace Applications.Mapping
{
    public class ServiceOrderMappingProfile : Profile
    {
        public ServiceOrderMappingProfile ( )
        {
            // Mapeamento completo com CurrentSectorName
            CreateMap<ServiceOrder, ServiceOrderDetailsDto> ( )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
                .ForMember ( dest => dest.CurrentSectorName, opt => opt.Ignore ( ) )
                .AfterMap ( ( src, dest ) =>
                {
                    var orderedStages = src.Stages
                        .OrderByDescending(s => s.DateIn)
                        .ToList();

                    if ( !orderedStages.Any ( ) )
                    {
                        dest.CurrentSectorName = null;
                        return;
                    }

                    if ( src.Status == OrderStatus.Production )
                    {
                        var openStage = orderedStages.FirstOrDefault(s => s.DateOut == null);
                        dest.CurrentSectorName = openStage?.Sector?.Name;
                        return;
                    }

                    if ( src.Status == OrderStatus.TryIn )
                    {
                        var lastBeforeTryIn = orderedStages
                            .Where(s => s.DateOut != null)
                            .OrderByDescending(s => s.DateOut)
                            .FirstOrDefault();

                        dest.CurrentSectorName = lastBeforeTryIn?.Sector?.Name;
                        return;
                    }

                    var lastClosed = orderedStages
                        .Where(s => s.DateOut != null)
                        .OrderByDescending(s => s.DateOut)
                        .FirstOrDefault();

                    dest.CurrentSectorName = lastClosed?.Sector?.Name;
                } );

            // Mapeamento para listagem simples (sem stages)
            CreateMap<ServiceOrder, ServiceOrderResponseDto> ( )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
                .ForMember ( dest => dest.CurrentSectorName, opt => opt.Ignore ( ) )
                .AfterMap ( ( src, dest ) =>
                {
                    var orderedStages = src.Stages
                        .OrderByDescending(s => s.DateIn)
                        .ToList();

                    if ( !orderedStages.Any ( ) )
                    {
                        dest.CurrentSectorName = null;
                        return;
                    }

                    if ( src.Status == OrderStatus.Production )
                    {
                        var openStage = orderedStages.FirstOrDefault(s => s.DateOut == null);
                        dest.CurrentSectorName = openStage?.Sector?.Name;
                        return;
                    }

                    if ( src.Status == OrderStatus.TryIn )
                    {
                        var lastBeforeTryIn = orderedStages
                            .Where(s => s.DateOut != null)
                            .OrderByDescending(s => s.DateOut)
                            .FirstOrDefault();

                        dest.CurrentSectorName = lastBeforeTryIn?.Sector?.Name;
                        return;
                    }

                    var lastClosed = orderedStages
                        .Where(s => s.DateOut != null)
                        .OrderByDescending(s => s.DateOut)
                        .FirstOrDefault();

                    dest.CurrentSectorName = lastClosed?.Sector?.Name;
                } );

            // Mapeamento para listagem enxuta
            CreateMap<ServiceOrder, ServiceOrderListDto> ( )
                .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) );

            CreateMap<ServiceOrder, ServiceOrderAlertDto> ( )
    .ForMember ( dest => dest.ClientName, opt => opt.MapFrom ( src => src.Client.ClientName ) )
    .ForMember ( dest => dest.Status, opt => opt.MapFrom ( src => src.Status.ToString ( ) ) )
    .ForMember ( dest => dest.CurrentSectorName, opt => opt.Ignore ( ) )
    .ForMember ( dest => dest.LastTryInDate, opt => opt.Ignore ( ) )
    .ForMember ( dest => dest.DaysOut, opt => opt.Ignore ( ) )
    .AfterMap ( ( src, dest ) =>
    {
        var lastStage = src.Stages
            .Where(s => s.DateOut != null)
            .OrderByDescending(s => s.DateOut)
            .FirstOrDefault();

        dest.CurrentSectorName = lastStage?.Sector?.Name ?? string.Empty;
        dest.LastTryInDate = lastStage?.DateOut ?? DateTime.MinValue;
        dest.DaysOut = lastStage?.DateOut is DateTime dt
            ? ( int ) ( DateTime.UtcNow - dt ).TotalDays
            : 0;
    } );

            CreateMap<CreateWorkDto, Work>();
CreateMap<CreateServiceOrderDto, ServiceOrder>()
    .ForMember(dest => dest.Works, opt => opt.MapFrom(src => src.Works));

        }
    }
}
