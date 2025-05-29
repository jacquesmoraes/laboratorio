using Core.Enums;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Models.ServiceOrders;

namespace Core.Models.Billing
{
    public class BillingInvoice
    {
        public int BillingInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public decimal TotalServiceOrdersAmount { get; set; }
        public decimal PreviousCredit { get; set; } = 0m;
        public decimal PreviousDebit { get; set; } = 0m;

        public decimal TotalInvoiceAmount => TotalServiceOrdersAmount + PreviousDebit - PreviousCredit;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;
        public List<ServiceOrder> ServiceOrders { get; set; } = [];
        public List<Payment> Payments { get; set; } = [];



    }


}
