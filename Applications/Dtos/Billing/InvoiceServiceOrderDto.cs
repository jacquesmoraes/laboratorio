namespace Applications.Dtos.Billing
{
    public class InvoiceServiceOrderDto
    {
        public DateTime DateIn  { get; set; }
        public string OrderCode  { get; set; } = string.Empty;
        public List<InvoiceWorkItemDto> Works { get; set; } = [];
        public decimal Subtotal { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime FinishedAt { get; set; }
       

        
    }

}
