namespace Applications.Dtos.Payments
{
    public class ClientPaymentDto
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        public int? BillingInvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
    }

}
