using Core.Models.Clients;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.PayementSpecifications
{
    public class PerClientPaymentSpecification : BaseSpecification<PerClientPayment>
    {
        public PerClientPaymentSpecification ( int id )
            : base ( x => x.Id == id )
        {
            AddInclude ( x => x.Client );
        }

        public PerClientPaymentSpecification ( Expression<Func<PerClientPayment, bool>> criteria, bool includeClient = false )
            : base ( criteria )
        {
            if ( includeClient )
                AddInclude ( x => x.Client );
        }




        public static class PerClientPaymentSpecs
        {
            public static PerClientPaymentSpecification ById ( int id ) => new ( id );

            public static PerClientPaymentSpecification ByClientId ( int clientId )
                => new ( x => x.ClientId == clientId, includeClient: true );
            public static PerClientPaymentSpecification ByClientAndDateRange ( int clientId, DateTime start, DateTime end )
                => new (
                    x => x.ClientId == clientId &&
                    x.PaymentDate >= start &&
                    x.PaymentDate < end,
                    includeClient: true );

        }


    }

}
