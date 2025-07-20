using Core.Models.Clients;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaDashboardSpecification ( Expression<Func<Client, bool>> criteria ) : BaseSpecification<Client>(criteria)
    {
        public static class ClientAreaDashboardSpecs
        {
            public static ClientAreaDashboardSpecification ForDashboard(int clientId)
            {
                var spec = new ClientAreaDashboardSpecification(c => c.ClientId == clientId);
                
                // Apenas o necessário para dashboard
                spec.AddInclude(c => c.Address);
                
                return spec;
            }
        }
    }
}