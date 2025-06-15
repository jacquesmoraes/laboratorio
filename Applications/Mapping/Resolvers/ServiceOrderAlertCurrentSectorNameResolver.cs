using Applications.Records.ServiceOrders;
using AutoMapper;
using Core.Enums;
using Core.Models.ServiceOrders;

namespace Applications.Mapping.Resolvers
{
    public class ServiceOrderAlertCurrentSectorNameResolver : IValueResolver<ServiceOrder, ServiceOrderAlertRecord, string>
{
    public string Resolve(ServiceOrder src, ServiceOrderAlertRecord dest, string member, ResolutionContext context)
    {
        var orderedStages = src.Stages.OrderByDescending(s => s.DateIn).ToList();
        if (!orderedStages.Any()) return string.Empty;

        return src.Status switch
        {
            OrderStatus.Production => orderedStages.FirstOrDefault(s => s.DateOut == null)?.Sector?.Name ?? string.Empty,
            OrderStatus.TryIn => orderedStages.Where(s => s.DateOut != null).OrderByDescending(s => s.DateOut).FirstOrDefault()?.Sector?.Name ?? string.Empty,
            _ => orderedStages.Where(s => s.DateOut != null).OrderByDescending(s => s.DateOut).FirstOrDefault()?.Sector?.Name ?? string.Empty
        };
    }
}
}
