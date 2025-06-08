using Core.Enums;
using Core.Models.Billing;

namespace Core.Models.Clients
{
    public class ClientBalance
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal Credit => TotalPaid > TotalInvoiced ? TotalPaid - TotalInvoiced : 0;
        public decimal Debit => TotalInvoiced > TotalPaid ? TotalInvoiced - TotalPaid : 0;
        public decimal Balance => TotalPaid - TotalInvoiced;

        public List<BillingInvoice> OpenInvoices { get; set; } = [];
        public List<BillingInvoice> ClosedInvoices { get; set; } = [];

        public BillingInvoice? CurrentOpenInvoice => OpenInvoices.OrderByDescending ( i => i.CreatedAt ).FirstOrDefault ( );

        public static ClientBalance Calculate ( Client client )
        {
            var allInvoices = client.ServiceOrders
            .Where(o => o.BillingInvoice != null)
            .Select(o => o.BillingInvoice!)
            .Distinct()
            .ToList();

            var payments = client.Payments;

            return new ClientBalance
            {
                TotalPaid = payments.Sum ( p => p.AmountPaid ),
                TotalInvoiced = allInvoices.Sum ( i => i.TotalServiceOrdersAmount ),
                OpenInvoices = allInvoices.Where ( i => i.Status == InvoiceStatus.Open || i.Status == InvoiceStatus.PartiallyPaid ).ToList ( ),
                ClosedInvoices = allInvoices.Where ( i => i.Status == InvoiceStatus.Closed ).ToList ( )
            };
        }
    }
}