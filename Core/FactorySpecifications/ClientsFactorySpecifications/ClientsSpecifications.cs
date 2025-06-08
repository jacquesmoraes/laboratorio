using Core.Models.Clients;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientsFactorySpecifications
{
    public class ClientSpecification : BaseSpecification<Client>
    {
        public ClientSpecification ( ) { }

        public ClientSpecification ( Expression<Func<Client, bool>> criteria )
            : base ( criteria ) { }

        public ClientSpecification ( int id )
            : base ( x => x.ClientId == id ) { }

        public ClientSpecification ( string? name )
            : base ( x => name == null || x.ClientName.Contains ( name ) ) { }

        public static class ClientSpecs
        {
            public static ClientSpecification All ( )
            {
                var spec = new ClientSpecification();
                spec.AddInclude ( x => x.TablePrice! );
                spec.AddInclude ( x => x.Address );
                spec.AddInclude ( x => x.Patients );
                spec.AddInclude ( x => x.Payments );
                spec.AddInclude ( x => x.ServiceOrders );
                return spec;
            }
            public static ClientSpecification ByIdFullForBalance ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( x => x.Payments );
                spec.AddInclude ( x => x.BillingInvoices );
                spec.AddInclude ( x => x.ServiceOrders );
                spec.AddInclude ( "ServiceOrders.BillingInvoice" );
                return spec;
            }

            public static ClientSpecification ById ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( x => x.TablePrice! );
                spec.AddInclude ( x => x.Address );
                spec.AddInclude ( x => x.Patients );
                spec.AddInclude ( x => x.Payments );
                spec.AddInclude ( x => x.ServiceOrders );


                return spec;
            }


            public static ClientSpecification ByIdWithInvoices ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( c => c.BillingInvoices );
                return spec;
            }


            public static ClientSpecification ByName ( string name )
            {
                var spec = new ClientSpecification(name);
                spec.AddInclude ( x => x.TablePrice! );
                return spec;
            }
        }


    }
}
