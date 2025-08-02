using Core.Models.Clients;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientsSpecifications
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

        public ClientSpecification ( QueryParams p )
          : base ( c =>
              string.IsNullOrEmpty ( p.Search ) ||
                 c.ClientName.ToLower ( ).Contains ( p.Search.ToLower ( ) ) ||
          ( c.ClientEmail != null && c.ClientEmail.ToLower ( ).Contains ( p.Search.ToLower ( ) ) ) ||
          ( c.ClientPhoneNumber != null && c.ClientPhoneNumber.Contains ( p.Search ) ) ||
          ( c.ClientCpf != null && c.ClientCpf.Contains ( p.Search ) ) ||
          ( c.Address.City != null && c.Address.City.ToLower ( ).Contains ( p.Search.ToLower ( ) ) ) )
        {
            AddInclude ( x => x.TablePrice! );
            AddInclude ( x => x.Address );
            AddInclude ( x => x.ServiceOrders ); // Para calcular IsInactive

            // Aplicar ordenação customizada para Client
            ApplyClientSorting ( p.Sort );

            // Aplicar paginação
            ApplyPaging ( ( p.PageNumber - 1 ) * p.PageSize, p.PageSize );
        }

        private void ApplyClientSorting ( string? sort )
        {
            if ( string.IsNullOrWhiteSpace ( sort ) )
            {
                // Ordenação padrão por nome do cliente
                AddOrderBy ( x => x.ClientName );
                return;
            }

            var descending = sort.EndsWith("Desc", StringComparison.OrdinalIgnoreCase);
            var propertyName = descending
                ? sort[..^4] // remove "Desc"
                : sort;

            // Mapear propriedades válidas para Client
            switch ( propertyName.ToLower ( ) )
            {
                case "clientname":
                    if ( descending )
                        AddOrderByDescending ( x => x.ClientName );
                    else
                        AddOrderBy ( x => x.ClientName );
                    break;
                case "clientemail":
                    if ( descending )
                        AddOrderByDescending ( x => x.ClientEmail ?? string.Empty );
                    else
                        AddOrderBy ( x => x.ClientEmail ?? string.Empty );
                    break;
                case "clientphone":
                case "clientphonenumber":
                    if ( descending )
                        AddOrderByDescending ( x => x.ClientPhoneNumber ?? string.Empty );
                    else
                        AddOrderBy ( x => x.ClientPhoneNumber ?? string.Empty );
                    break;
                case "city":
                    if ( descending )
                        AddOrderByDescending ( x => x.Address.City ?? string.Empty );
                    else
                        AddOrderBy ( x => x.Address.City ?? string.Empty );
                    break;
                case "billingmode":
                    if ( descending )
                        AddOrderByDescending ( x => x.BillingMode );
                    else
                        AddOrderBy ( x => x.BillingMode );
                    break;

                default:
                    // Fallback para ordenação por nome
                    AddOrderBy ( x => x.ClientName );
                    break;
            }
        }

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
                spec.AddInclude ( "Payments.BillingInvoice" );


                return spec;
            }
            public static ClientSpecification AllForForm ( )
            {
                var spec = new ClientSpecification();
                spec.AddInclude ( x => x.TablePrice! );
                spec.AddInclude ( x => x.Address );
                return spec;
            }

            public static ClientSpecification AllForForm ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( x => x.TablePrice! );
                spec.AddInclude ( x => x.Address );
                return spec;
            }



            public static ClientSpecification ByIdWithInvoices ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( c => c.BillingInvoices );
                return spec;
            }

            public static ClientSpecification ByIdWithTablePriceItems ( int id )
            {
                var spec = new ClientSpecification(id);
                spec.AddInclude ( x => x.TablePrice! );
                spec.AddInclude ( "TablePrice.Items" );
                spec.AddInclude ( "TablePrice.Items.WorkType" );
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
