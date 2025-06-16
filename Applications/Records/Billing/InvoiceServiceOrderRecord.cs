namespace Applications.Records.Billing
{
    public record InvoiceServiceOrderRecord
    {
        public DateTime DateIn { get; init; }
        public string OrderCode { get; init; } = string.Empty;
        public List<InvoiceWorkItemRecord> Works { get; init; } = [];
        public decimal Subtotal { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public DateTime? FinishedAt { get; init; }
    }
}
