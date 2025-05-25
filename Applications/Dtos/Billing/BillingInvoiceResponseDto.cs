using Applications.Dtos.Clients;

namespace Applications.Dtos.Billing
{
    public class BillingInvoiceResponseDto
    {
        public int BillingInvoiceId { get; set; }
        public ClientResponseForOrderServiceDto Client { get; set; } = new ( );
        public DateTime IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public List<InvoiceServiceOrderDto> ServiceOrders { get; set; } = [];
    }

}
