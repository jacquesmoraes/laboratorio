using Core.Models.Production;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaServiceOrderSpecification ( Expression<Func<ServiceOrder, bool>> criteria ) : BaseSpecification<ServiceOrder>(criteria)
    {
        public static class ClientAreaServiceOrderSpecs
        {
            public static ClientAreaServiceOrderSpecification ByIdForDetails(int serviceOrderId, int clientId)
            {
                Expression<Func<ServiceOrder, bool>> criteria = so =>
                    so.ServiceOrderId == serviceOrderId &&
                    so.ClientId == clientId;

                var spec = new ClientAreaServiceOrderSpecification(criteria);

                // Apenas includes necessários para o projection
                spec.AddInclude(o => o.Works);
                spec.AddInclude($"{nameof(ServiceOrder.Works)}.{nameof(Work.WorkType)}");
                spec.AddInclude($"{nameof(ServiceOrder.Works)}.{nameof(Work.Shade)}");
                spec.AddInclude($"{nameof(ServiceOrder.Works)}.{nameof(Work.Shade)}.{nameof(Shade.Scale)}");
                spec.AddInclude(o => o.Stages);
                spec.AddInclude($"{nameof(ServiceOrder.Stages)}.{nameof(ProductionStage.Sector)}");

                return spec;
            }

            public static ClientAreaServiceOrderSpecification Paged(ServiceOrderParams p)
            {
                // Aplicar filtros
                Expression<Func<ServiceOrder, bool>> criteria = so =>
                    so.ClientId == p.ClientId &&
                    (!p.Status.HasValue || so.Status == p.Status) &&
                    (string.IsNullOrEmpty(p.Search) ||
                     so.PatientName.ToLower().Contains(p.Search.ToLower()));

                var spec = new ClientAreaServiceOrderSpecification(criteria);

                // Apenas includes necessários para busca por nome do cliente
                if (!string.IsNullOrEmpty(p.Search))
                {
                    spec.AddInclude(so => so.Client);
                }

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
                     so.PatientName.ToLower().Contains(p.Search.ToLower()));

                var spec = new ClientAreaServiceOrderSpecification(criteria);
                
                // Apenas include necessário para busca por nome do cliente
                if (!string.IsNullOrEmpty(p.Search))
                {
                    spec.AddInclude(so => so.Client);
                }

                return spec;
            }
        }
    }
}