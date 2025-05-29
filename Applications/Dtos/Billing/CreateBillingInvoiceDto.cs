namespace Applications.Dtos.Billing
{
    public class CreateBillingInvoiceDto
    {
        public int ClientId { get; set; }
        public List<int> ServiceOrderIds { get; set; } = [];
        public string? Description { get; set; }  
    }
}
