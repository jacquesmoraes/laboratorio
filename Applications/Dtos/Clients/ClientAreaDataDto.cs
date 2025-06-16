using Applications.Projections.Billing;
using Applications.Records.Payments;
using Core.Models.Clients;

namespace Applications.Dtos.Clients
{
    public class ClientAreaDataDto
    {
        public Client Client { get; init; } = default!;
        public decimal TotalPaid { get; init; }
        public decimal TotalInvoiced { get; init; }
        public decimal Credit { get; init; }
        public decimal Debit { get; init; }
         public List<ClientPaymentRecord> Payments { get; set; } = [];
        public List<BillingInvoiceResponseProjection> Invoices { get; init; } = [];
    }
}
