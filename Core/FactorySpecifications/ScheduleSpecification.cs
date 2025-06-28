using Core.Models.Schedule;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications
{
    public class ScheduleSpecification : BaseSpecification<ServiceOrderSchedule>
    {
        public ScheduleSpecification ( ) { }

        public ScheduleSpecification ( Expression<Func<ServiceOrderSchedule, bool>> criteria )
            : base ( criteria ) { }

        public static class ScheduleSpecs
        {
            public static ScheduleSpecification ById ( int id )
                => new ( s => s.Id == id );

            public static ScheduleSpecification BySectorAndDate ( int sectorId, DateTime date )
            {
                var spec = new ScheduleSpecification(s =>
                    s.SectorId == sectorId &&
                    s.ScheduledDate != null &&
                    s.ScheduledDate.Value.Date == date.Date &&
                    !s.IsDelivered);

                AddIncludes ( spec );
                spec.AddOrderBy ( s => s.ScheduledDate ?? DateTime.MaxValue );
                return spec;
            }

            public static ScheduleSpecification ByCurrentSectorAndDate ( int sectorId, DateTime date )
            {
                var start = date.Date;
                var end = start.AddDays(1);

                var spec = new ScheduleSpecification(s =>
                s.ScheduledDate >= start &&
                s.ScheduledDate < end &&
                !s.IsDelivered &&
                s.ServiceOrder.Stages.Any(stage =>
                stage.DateOut == null && stage.SectorId == sectorId
                ));

                AddIncludes ( spec );
                spec.AddOrderBy ( s => s.ScheduledDate ?? DateTime.MaxValue );
                return spec;
            }

            public static ScheduleSpecification OverdueDeliveries ( )
            {
                var todayUtc = DateTime.UtcNow.Date;

                return new ScheduleSpecification ( s =>
                    s.ScheduledDate != null &&
                    s.ScheduledDate < todayUtc &&
                    !s.IsDelivered &&
                    !s.IsOverdue
                );
            }


            public static ScheduleSpecification ForDate ( DateTime date )
            {
                var start = date.Date;
                var end = start.AddDays(1);

                var spec = new ScheduleSpecification(s =>
                s.ScheduledDate != null &&
                s.ScheduledDate >= start &&
                s.ScheduledDate < end &&
                !s.IsDelivered);

                AddIncludes ( spec );
                spec.AddOrderBy ( s => s.ScheduledDate ?? DateTime.MaxValue );

                return spec;
            }


            public static ScheduleSpecification ActiveByServiceOrderId ( int serviceOrderId )
            {
                return new ScheduleSpecification ( s =>
                    s.ServiceOrderId == serviceOrderId &&
                    !s.IsDelivered );
            }

            private static void AddIncludes ( ScheduleSpecification spec )
            {
                spec.AddInclude ( s => s.ServiceOrder );
                spec.AddInclude ( s => s.ServiceOrder.Client );
                spec.AddInclude ( s => s.Sector! );
                spec.AddInclude ( "ServiceOrder.Stages.Sector" ); // ✅ garante o setor atual

            }
        }
    }
}
