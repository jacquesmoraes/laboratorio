using Core.Enums;
using Core.Models.Clients;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ServiceOrderFactorySpecifications
{
    public class ServiceOrderSpecification : BaseSpecification<ServiceOrder>
    {
        public ServiceOrderSpecification ( ) { }

        public ServiceOrderSpecification ( Expression<Func<ServiceOrder, bool>> criteria )
            : base ( criteria ) { }

        public ServiceOrderSpecification ( int id )
            : base ( o => o.ServiceOrderId == id ) { }

        public static class ServiceOrderSpecs
        {
            public static ServiceOrderSpecification ByIdFull ( int id )
            {
                var spec = new ServiceOrderSpecification(id);

                spec.AddInclude ( o => o.Client );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Client )}.{nameof ( Client.Balance )}" );

                spec.AddInclude ( o => o.Works );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.WorkType )}" );

                spec.AddInclude ( o => o.Stages );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Stages )}.{nameof ( ProductionStage.Sector )}" );

                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}" );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}.{nameof ( Shade.Scale )}" );


                return spec;
            }


            public static ServiceOrderSpecification ByIds ( List<int> ids )
                => new ( x => ids.Contains ( x.ServiceOrderId ) );

            public static ServiceOrderSpecification FinishedInMonthByClient ( int clientId, DateTime start, DateTime end )
                => new ( x =>
                    x.ClientId == clientId &&
                    x.Status == OrderStatus.Finished &&
                    x.DateIn >= start &&
                    x.DateIn < end );

            public static ServiceOrderSpecification OutForTryInMoreThanXDays ( int days )
            {
                var limite = DateTime.UtcNow.AddDays(-days);

                Expression<Func<ServiceOrder, bool>> criteria = o =>
                o.Status == OrderStatus.TryIn &&
                o.Stages.Any(s => s.DateOut != null && s.DateOut < limite);

                var spec = new ServiceOrderSpecification(criteria);

                spec.AddInclude ( o => o.Client );
                spec.AddInclude ( o => o.Stages );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Stages )}.{nameof ( ProductionStage.Sector )}" );

                return spec;
            }

            public static ServiceOrderSpecification Filtered ( string? status, int? clientId, DateTime? start, DateTime? end )
            {
                Expression<Func<ServiceOrder, bool>> criteria = x =>
                    (string.IsNullOrEmpty(status) || x.Status.ToString() == status) &&
                    (!clientId.HasValue || x.ClientId == clientId.Value) &&
                    (!start.HasValue || x.DateIn >= start.Value) &&
                    (!end.HasValue || x.DateIn <= end.Value);

                var spec = new ServiceOrderSpecification(criteria);

                // Includes necessários para o ResponseDto
                spec.AddInclude ( o => o.Client );
                spec.AddInclude ( o => o.Works );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.WorkType )}" );
                spec.AddInclude ( o => o.Stages );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Stages )}.{nameof ( ProductionStage.Sector )}" );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}" );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}.{nameof ( Shade.Scale )}" );

                return spec;
            }
        }
    }
}
