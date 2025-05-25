namespace Applications.Dtos.Billing
{
    public class InvoiceWorkItemDto
    {
        public string WorkTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal PriceTotal => Quantity * PriceUnit;
    }

}
