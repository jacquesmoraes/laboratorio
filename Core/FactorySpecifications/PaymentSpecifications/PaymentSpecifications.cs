using Core.Models.Payments;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.PaymentSpecifications
{
    public class PaymentSpecification : BaseSpecification<Payment>
    {
        public PaymentSpecification ( Expression<Func<Payment, bool>> criteria, bool includeClient = false )
            : base ( criteria )
        {
            if ( includeClient )
                AddInclude ( x => x.Client );
        }

        public static class PaymentSpecs
        {
            public static PaymentSpecification ById ( int id, bool includeClient = false )
                => new ( p => p.Id == id, includeClient );

            public static PaymentSpecification ByClientId ( int clientId, bool includeClient = false )
                => new ( p => p.ClientId == clientId, includeClient );

            public static PaymentSpecification ByInvoiceId ( int invoiceId, bool includeClient = false )
                => new ( p => p.BillingInvoiceId == invoiceId, includeClient );

            public static PaymentSpecification Paged ( PaymentParams p )
            {
                Expression<Func<Payment, bool>> criteria = x =>
                    (!p.ClientId.HasValue || x.ClientId == p.ClientId) &&
                    (string.IsNullOrEmpty(p.Search) || x.Description!.ToLower().Contains(p.Search.ToLower())) &&
                    (!p.StartDate.HasValue || x.PaymentDate >= p.StartDate.Value) &&
                    (!p.EndDate.HasValue || x.PaymentDate <= p.EndDate.Value);

                var spec = new PaymentSpecification(criteria);

                spec.AddInclude ( x => x.Client );
                spec.AddInclude ( x => x.BillingInvoice );

                spec.ApplySorting ( p.Sort );
                spec.ApplyPaging ( ( p.PageNumber - 1 ) * p.PageSize, p.PageSize );

                return spec;
            }

            public static PaymentSpecification PagedForCount ( PaymentParams p )
            {
                Expression<Func<Payment, bool>> criteria = x =>
        (!p.ClientId.HasValue || x.ClientId == p.ClientId) &&
        (string.IsNullOrEmpty(p.Search) || x.Description!.ToLower().Contains(p.Search.ToLower())) &&
        (!p.StartDate.HasValue || x.PaymentDate >= p.StartDate.Value) &&
        (!p.EndDate.HasValue || x.PaymentDate <= p.EndDate.Value);

                return new PaymentSpecification ( criteria );
            }

        }
    }
}
