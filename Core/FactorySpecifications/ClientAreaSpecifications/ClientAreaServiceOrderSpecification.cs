using Core.Models.ServiceOrders;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaServiceOrderSpecification : BaseSpecification<ServiceOrder>
    {
        public ClientAreaServiceOrderSpecification(Expression<Func<ServiceOrder, bool>> criteria)
            : base(criteria)
        {
        }

        public static class ClientAreaServiceOrderSpecs
        {
           public static ClientAreaServiceOrderSpecification Paged(ServiceOrderParams p)
            {
                // Aplicar todos os filtros como na especificação normal
                Expression<Func<ServiceOrder, bool>> criteria = so =>
                    so.ClientId == p.ClientId &&
                    (!p.Status.HasValue || so.Status == p.Status) &&
                    (string.IsNullOrEmpty(p.Search) ||
                     so.PatientName.ToLower().Contains(p.Search.ToLower()) ||
                     so.Client.ClientName.ToLower().Contains(p.Search.ToLower()));

                var spec = new ClientAreaServiceOrderSpecification(criteria);

                spec.AddInclude(so => so.Client);
                spec.AddInclude(so => so.BillingInvoice!);
                
                // Apenas includes necessários para client area
                spec.AddInclude(so => so.Works);
                spec.AddInclude(so => so.Stages);
                spec.AddInclude("Stages.Sector"); 

                spec.ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);
                spec.ApplySorting(p.Sort);
                spec.AddOrderByDescending(so => so.DateIn);

                return spec;
            }

            public static ClientAreaServiceOrderSpecification PagedForCount(ServiceOrderParams p)
            {
                // Aplicar os mesmos filtros para o count
                Expression<Func<ServiceOrder, bool>> criteria = so =>
                    so.ClientId == p.ClientId &&
                    (!p.Status.HasValue || so.Status == p.Status) &&
                    (string.IsNullOrEmpty(p.Search) ||
                     so.PatientName.ToLower().Contains(p.Search.ToLower()) ||
                     so.Client.ClientName.ToLower().Contains(p.Search.ToLower()));

                var spec = new ClientAreaServiceOrderSpecification(criteria);
                spec.AddInclude(so => so.Client); 

                return spec;
            }
        }
    }
}