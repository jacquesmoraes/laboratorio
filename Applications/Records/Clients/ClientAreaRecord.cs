using Applications.Projections.Billing;
using Applications.Records.Payments;

namespace Applications.Records.Clients
{
    public record ClientAreaRecord
    {
        public string ClientName { get; init; } = string.Empty;
        public string? Street { get; init; }
        public int? Number { get; init; }
        public string? Complement { get; init; }
        public string? Neighborhood { get; init; }
        public string? City { get; init; }
        public string? PhoneNumber { get; init; }

        public decimal TotalPaid { get; init; }
        public decimal TotalInvoiced { get; init; }
        public decimal Credit { get; init; }
        public decimal Debit { get; init; }
        public decimal Balance => TotalPaid - TotalInvoiced;
        public IReadOnlyList<ClientPaymentRecord> Payments { get; init; } = [];
        public List<BillingInvoiceResponseProjection> Invoices { get; init; } = [];
    }
}