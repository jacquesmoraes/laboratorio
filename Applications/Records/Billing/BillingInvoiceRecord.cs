using Applications.Records.Clients;

namespace Applications.Records.Billing
{
    public record BillingInvoiceRecord
    {
        public int BillingInvoiceId { get; init; }
        public string InvoiceNumber { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public string Description { get; init; } = string.Empty;
        public ClientInvoiceRecord Client { get; init; } = default!;
        public List<InvoiceServiceOrderRecord> ServiceOrders { get; init; } = [];
        public decimal TotalServiceOrdersAmount { get; init; }
        public decimal PreviousCredit { get; init; }
        public decimal PreviousDebit { get; init; }
        public decimal TotalInvoiceAmount { get; init; }
    }
}
