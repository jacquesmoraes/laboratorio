using Core.Enums;
using Core.Models.Clients;
using Core.Models.ServiceOrders;

namespace Core.Models.Billing
{
    public class BillingInvoice
    {
        public int BillingInvoiceId { get; set; }
        public int ClientId { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;

        public Client Client { get; set; } = null!;
        public List<ServiceOrder> ServiceOrders { get; set; } = [];
    }

}
