using Core.Models.Payments;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.PayementSpecifications
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
            public static PaymentSpecification ByClientIdWithInvoice ( int clientId )
                => new ( p => p.ClientId == clientId )
                {
                    Includes = { p => p.Client!, p => p.BillingInvoice! }
                };

            public static PaymentSpecification ByClientAndDateRange ( int clientId, DateTime start, DateTime end, bool includeClient = false )
                => new ( p =>
                    p.ClientId == clientId &&
                    p.PaymentDate >= start &&
                    p.PaymentDate < end,
                    includeClient );
        }
    }
}
