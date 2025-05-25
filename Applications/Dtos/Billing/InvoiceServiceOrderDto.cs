namespace Applications.Dtos.Billing
{
    public class InvoiceServiceOrderDto
    {
        public DateTime EntryDate { get; set; }
        public string OrderNumberFormatted { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime FinishedAt { get; set; }
        public DateTime? DeliveryDate { get; set; } // opcional
        public decimal Subtotal { get; set; }

        public List<InvoiceWorkItemDto> Items { get; set; } = [];
    }

}
