using Core.Models.Payments;
using Core.Params;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.ClientAreaSpecifications
{
    public class ClientAreaPaymentSpecification : BaseSpecification<Payment>
    {
        public ClientAreaPaymentSpecification(Expression<Func<Payment, bool>> criteria)
            : base(criteria)
        {
        }

        public static class ClientAreaPaymentSpecs
        {
            public static ClientAreaPaymentSpecification Paged(PaymentParams p)
            {
                Expression<Func<Payment, bool>> criteria = x =>
                    (!p.ClientId.HasValue || x.ClientId == p.ClientId) &&
                    (string.IsNullOrEmpty(p.Search) || x.Description!.ToLower().Contains(p.Search.ToLower())) &&
                    (!p.StartDate.HasValue || x.PaymentDate >= p.StartDate.Value) &&
                    (!p.EndDate.HasValue || x.PaymentDate <= p.EndDate.Value);

                var spec = new ClientAreaPaymentSpecification(criteria);

                // Apenas o include necessário para client area
                spec.AddInclude(x => x.BillingInvoice!);

                if (!string.IsNullOrEmpty(p.Sort))
                {
                    spec.ApplySorting(p.Sort);
                }
                else
                {
                    spec.ApplySorting("-PaymentDate");
                }

                spec.ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

                return spec;
            }

            public static ClientAreaPaymentSpecification PagedForCount(PaymentParams p)
            {
                Expression<Func<Payment, bool>> criteria = x =>
                    (!p.ClientId.HasValue || x.ClientId == p.ClientId) &&
                    (string.IsNullOrEmpty(p.Search) || x.Description!.ToLower().Contains(p.Search.ToLower())) &&
                    (!p.StartDate.HasValue || x.PaymentDate >= p.StartDate.Value) &&
                    (!p.EndDate.HasValue || x.PaymentDate <= p.EndDate.Value);

                return new ClientAreaPaymentSpecification(criteria);
            }
        }
    }
}