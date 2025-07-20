using Core.Enums;
using Core.Models.Billing;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaBillingInvoiceSpecification : BaseSpecification<BillingInvoice>
    {
        public ClientAreaBillingInvoiceSpecification(Expression<Func<BillingInvoice, bool>> criteria)
            : base(criteria)
        {
        }

        public static class ClientAreaBillingInvoiceSpecs
        {
            public static ClientAreaBillingInvoiceSpecification Paged(InvoiceParams p)
            {
                Expression<Func<BillingInvoice, bool>> criteria = i =>
                    (!p.ClientId.HasValue || i.ClientId == p.ClientId) &&
                    (!p.Status.HasValue || i.Status == p.Status) &&
                    (!p.StartDate.HasValue || i.CreatedAt >= p.StartDate.Value) &&
                    (!p.EndDate.HasValue || i.CreatedAt <= p.EndDate.Value) ;
                   

                var spec = new ClientAreaBillingInvoiceSpecification(criteria);

                // Apenas includes necessários para client area
                spec.AddInclude(i => i.Payments);

                // Paginação
                if (!string.IsNullOrEmpty(p.Sort))
                {
                    spec.ApplySorting(p.Sort);
                }
                else
                {
                    spec.ApplySorting("-CreatedAt");
                }

                spec.ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

                return spec;
            }

            public static ClientAreaBillingInvoiceSpecification PagedForCount(InvoiceParams p)
            {
                Expression<Func<BillingInvoice, bool>> criteria = i =>
                    (!p.ClientId.HasValue || i.ClientId == p.ClientId) &&
                    (!p.Status.HasValue || i.Status == p.Status) &&
                    (!p.StartDate.HasValue || i.CreatedAt >= p.StartDate.Value) &&
                    (!p.EndDate.HasValue || i.CreatedAt <= p.EndDate.Value) ;
                    

                return new ClientAreaBillingInvoiceSpecification(criteria);
            }
        }
    }
}