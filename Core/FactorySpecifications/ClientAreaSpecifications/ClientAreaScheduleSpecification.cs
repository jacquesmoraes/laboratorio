using Core.Enums;
using Core.Models.Schedule;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaScheduleSpecification ( Expression<Func<ServiceOrderSchedule, bool>> criteria ) : BaseSpecification<ServiceOrderSchedule> ( criteria )
    {
        public static class ClientAreaScheduleSpecs
        {
            public static ClientAreaScheduleSpecification UpcomingDeliveriesByClient ( int clientId )
            {
                var today = DateTime.Today;

                var spec = new ClientAreaScheduleSpecification(s =>
                s.ServiceOrder.ClientId == clientId &&
                s.ScheduledDate.HasValue &&
                s.ScheduledDate.Value >= today &&
                !s.IsDelivered &&
                (s.DeliveryType == ScheduledDeliveryType.TryIn ||
                s.DeliveryType == ScheduledDeliveryType.FinalDelivery));

                spec.AddInclude ( s => s.ServiceOrder );
                spec.AddOrderBy ( s => s.ScheduledDate!.Value );

                return spec;
            }
        }
    }
}