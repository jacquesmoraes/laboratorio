using Core.Models.Billing;
using Core.Models.ServiceOrders;
using Core.Specifications;
using System.Linq.Expressions;

namespace Core.FactorySpecifications.Billing
{
     public class BillingInvoiceSpecification : BaseSpecification<BillingInvoice>
    {
        public BillingInvoiceSpecification() { }

        public BillingInvoiceSpecification(Expression<Func<BillingInvoice, bool>> criteria)
            : base(criteria) { }

        public BillingInvoiceSpecification(int id)
            : base(i => i.BillingInvoiceId == id) { }

        public static class BillingInvoiceSpecs
        {
            public static BillingInvoiceSpecification ByIdFull(int id)
            {
                var spec = new BillingInvoiceSpecification(id);

                spec.AddInclude(i => i.Client);
                spec.AddInclude(i => i.ServiceOrders);
                spec.AddInclude($"{nameof(BillingInvoice.ServiceOrders)}.{nameof(ServiceOrder.Works)}");
                spec.AddInclude($"{nameof(BillingInvoice.ServiceOrders)}.{nameof(ServiceOrder.Works)}.{nameof(Core.Models.Works.Work.WorkType)}");
                
                return spec;
            }

            public static BillingInvoiceSpecification AllByClient(int clientId)
            {
                var spec = new BillingInvoiceSpecification(i => i.ClientId == clientId);
                spec.AddInclude(i => i.ServiceOrders);
                return spec;
            }
        }
    }
}
