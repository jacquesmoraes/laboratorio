using Core.Enums;
using Core.Models.Billing;
using Core.Models.Payments;

namespace Core.Models.Clients
{
    public class ClientBalance
    {
        public decimal TotalPaid { get; private set; }
        public decimal TotalInvoiced { get; private set; }

        public decimal Credit => TotalPaid > TotalInvoiced ? TotalPaid - TotalInvoiced : 0;
        public decimal Debit => TotalInvoiced > TotalPaid ? TotalInvoiced - TotalPaid : 0;
        public decimal Balance => TotalPaid - TotalInvoiced;

        public List<BillingInvoice> OpenInvoices { get; private set; } = [];
        public List<BillingInvoice> ClosedInvoices { get; private set; } = [];

        public BillingInvoice? CurrentOpenInvoice =>
            OpenInvoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault();

       
        public static ClientBalance Calculate(Client client)
        {
            var allInvoices = client.ServiceOrders
                .Where(o => o.BillingInvoice != null)
                .Select(o => o.BillingInvoice!)
                .Distinct()
                .ToList();

            return FromLists(client.Payments, allInvoices);
        }

        // test method
        public static ClientBalance FromLists(IEnumerable<Payment> payments, IEnumerable<BillingInvoice> invoices)
        {
            return new ClientBalance
            {
                TotalPaid = payments.Sum(p => p.AmountPaid),
                TotalInvoiced = invoices.Sum(i => i.TotalServiceOrdersAmount),
                
                OpenInvoices = invoices
                    .Where(i => i.Status == InvoiceStatus.Open || i.Status == InvoiceStatus.PartiallyPaid)
                    .ToList(),
                ClosedInvoices = invoices
                    .Where(i => i.Status == InvoiceStatus.Closed)
                    .ToList()
            };
        }
    }
}
