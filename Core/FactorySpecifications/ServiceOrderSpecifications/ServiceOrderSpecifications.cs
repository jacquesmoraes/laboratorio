using Core.Enums;
using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Specifications;
using Core.Params;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Core.FactorySpecifications.ServiceOrderSpecifications
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
            public static BaseSpecification<ServiceOrder> AllByClient ( int clientId )
                => new ( spec => spec.ClientId == clientId );

            public static ServiceOrderSpecification ByIdFull ( int id )
            {
                var spec = new ServiceOrderSpecification(id);
                AddFullIncludes ( spec );
                return spec;
            }

            public static ServiceOrderSpecification AvailableForScheduling ( )
            {
                var spec = new ServiceOrderSpecification(o => o.Status != OrderStatus.Finished);
                AddFullIncludes ( spec );
                return spec;
            }
            public static ServiceOrderSpecification ByIds ( List<int> ids )
            {
                var spec = new ServiceOrderSpecification(o => ids.Contains(o.ServiceOrderId));
                AddWorksIncludes ( spec );
                return spec;
            }

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
                AddStagesIncludes ( spec );

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
                AddFullIncludes ( spec );

                return spec;
            }

            // ✅ NOVO: Paginação, filtro e ordenação dinâmicos
            public static ServiceOrderSpecification Paged ( ServiceOrderParams p )
            {
                Expression<Func<ServiceOrder, bool>> criteria = o =>
                (!p.ClientId.HasValue || o.ClientId == p.ClientId) &&
                (!p.Status.HasValue || o.Status == p.Status) &&
                (!p.ExcludeFinished || o.Status != OrderStatus.Finished) &&
                (string.IsNullOrEmpty(p.Search) ||
                o.PatientName.ToLower().Contains(p.Search.ToLower()) ||
                o.Client.ClientName.ToLower().Contains(p.Search.ToLower())) &&
                (!p.StartDate.HasValue || o.DateIn >= p.StartDate.Value) &&
                (!p.EndDate.HasValue || o.DateIn <= p.EndDate.Value)&&
                (!p.ExcludeInvoiced || o.BillingInvoiceId == null);
                 


                var spec = new ServiceOrderSpecification(criteria);

                spec.AddInclude ( o => o.Client );
                spec.AddInclude ( o => o.Works );
                spec.AddInclude ( o => o.Stages );
                spec.AddInclude ( "Stages.Sector" );
                spec.ApplySorting ( p.Sort );
                spec.ApplyPaging ( ( p.PageNumber - 1 ) * p.PageSize, p.PageSize );

                return spec;
            }

            public static ServiceOrderSpecification PagedForCount ( ServiceOrderParams p )
            {
                Expression<Func<ServiceOrder, bool>> criteria = o =>
        (!p.ClientId.HasValue || o.ClientId == p.ClientId) &&
        (!p.Status.HasValue || o.Status == p.Status) &&
        (!p.ExcludeFinished || o.Status != OrderStatus.Finished) &&
        (string.IsNullOrEmpty(p.PatientName) || o.PatientName.ToLower().Contains(p.PatientName.ToLower())) &&
         (string.IsNullOrEmpty(p.ClientName) || o.Client.ClientName.ToLower().Contains(p.ClientName.ToLower())) &&
        (!p.StartDate.HasValue || o.DateIn >= p.StartDate.Value) &&
        (!p.EndDate.HasValue || o.DateIn <= p.EndDate.Value);

                return new ServiceOrderSpecification ( criteria );
            }


            // Métodos auxiliares
            private static void AddFullIncludes ( ServiceOrderSpecification spec )
            {
                spec.AddInclude ( o => o.Client );
                AddWorksIncludes ( spec );
                AddStagesIncludes ( spec );
            }

            private static void AddWorksIncludes ( ServiceOrderSpecification spec )
            {
                spec.AddInclude ( o => o.Works );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.WorkType )}" );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}" );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Works )}.{nameof ( Work.Shade )}.{nameof ( Shade.Scale )}" );
            }

            private static void AddStagesIncludes ( ServiceOrderSpecification spec )
            {
                spec.AddInclude ( o => o.Stages );
                spec.AddInclude ( $"{nameof ( ServiceOrder.Stages )}.{nameof ( ProductionStage.Sector )}" );
            }
        }
    }
}
