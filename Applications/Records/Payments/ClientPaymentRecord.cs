namespace Applications.Records.Payments
{
    public record ClientPaymentRecord
    {
        public int Id { get; init; }
        public DateTime PaymentDate { get; init; }
        public decimal AmountPaid { get; init; }
        public string? Description { get; init; }
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public int? BillingInvoiceId { get; init; }
        public string? InvoiceNumber { get; init; }

    }

}
