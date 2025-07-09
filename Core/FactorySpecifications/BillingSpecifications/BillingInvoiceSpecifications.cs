using Core.Enums;
using Core.Models.Billing;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.BillingSpecifications
{
    public class BillingInvoiceSpecification : BaseSpecification<BillingInvoice>
    {
        public BillingInvoiceSpecification ( ) { }

        public BillingInvoiceSpecification ( Expression<Func<BillingInvoice, bool>> criteria )
            : base ( criteria ) { }

        public BillingInvoiceSpecification ( int id )
            : base ( i => i.BillingInvoiceId == id ) { }

        public static class BillingInvoiceSpecs
        {
            public static BillingInvoiceSpecification ByIdFull ( int id )
            {
                var spec = new BillingInvoiceSpecification(id);

                spec.AddInclude ( i => i.Client );
                spec.AddInclude ( i => i.ServiceOrders );
                spec.AddInclude ( $"{nameof ( BillingInvoice.ServiceOrders )}.{nameof ( ServiceOrder.Works )}" );
                spec.AddInclude ( $"{nameof ( BillingInvoice.ServiceOrders )}.{nameof ( ServiceOrder.Works )}.{nameof ( WorkType )}" );
                spec.AddInclude ( i => i.Payments );
                return spec;
            }
            public static BillingInvoiceSpecification OpenOrPartiallyPaidByClient ( int clientId )
            {
                return new BillingInvoiceSpecification ( i =>
                    i.ClientId == clientId &&
                    ( i.Status == InvoiceStatus.Open || i.Status == InvoiceStatus.PartiallyPaid ) );
            }

            public static BillingInvoiceSpecification AllByClientFull ( int clientId )
            {
                var spec = new BillingInvoiceSpecification(i => i.ClientId == clientId);

                spec.AddInclude ( i => i.Client );
                spec.AddInclude ( "Client.Address" );
                spec.AddInclude ( i => i.ServiceOrders );
                spec.AddInclude ( "ServiceOrders.Works" );
                spec.AddInclude ( "ServiceOrders.Works.WorkType" );
                spec.AddInclude ( i => i.Payments );

                return spec;
            }

            public static BillingInvoiceSpecification AllByClient ( int clientId )
            {
                var spec = new BillingInvoiceSpecification(i => i.ClientId == clientId);
                spec.AddInclude ( i => i.ServiceOrders );
                spec.AddInclude ( i => i.Payments );
                return spec;
            }
            public static BillingInvoiceSpecification Paged ( InvoiceParams p )
            {
                Expression<Func<BillingInvoice, bool>> criteria = i =>
        (!p.ClientId.HasValue || i.ClientId == p.ClientId) &&
        (!p.Status.HasValue || i.Status == p.Status) &&
        (!p.StartDate.HasValue || i.CreatedAt >= p.StartDate.Value) &&
        (!p.EndDate.HasValue || i.CreatedAt <= p.EndDate.Value) &&
        (string.IsNullOrEmpty(p.Search) || 
         i.InvoiceNumber.ToLower().Contains(p.Search.ToLower()) ||
         i.Client.ClientName.ToLower().Contains(p.Search.ToLower()));

                var spec = new BillingInvoiceSpecification(criteria);

                // Includes padrão
                spec.AddInclude ( i => i.Payments );
                spec.AddInclude ( i => i.Client );
                spec.AddInclude ( "Client.Address" );

                // Paginação
                spec.ApplySorting ( p.Sort );
                spec.ApplyPaging ( ( p.PageNumber - 1 ) * p.PageSize, p.PageSize );

                return spec;
            }


            public static BillingInvoiceSpecification PagedForCount ( InvoiceParams p )
            {
                Expression<Func<BillingInvoice, bool>> criteria = i =>
        (!p.ClientId.HasValue || i.ClientId == p.ClientId) &&
        (!p.Status.HasValue || i.Status == p.Status) &&
        (!p.StartDate.HasValue || i.CreatedAt >= p.StartDate.Value) &&
        (!p.EndDate.HasValue || i.CreatedAt <= p.EndDate.Value) &&
        (string.IsNullOrEmpty(p.Search) || 
         i.InvoiceNumber.ToLower().Contains(p.Search.ToLower()) ||
         i.Client.ClientName.ToLower().Contains(p.Search.ToLower()));

                return new BillingInvoiceSpecification ( criteria );
            }

        }
    }
}
