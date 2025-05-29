using Core.Models.Billing;
using Core.Models.Clients;

namespace Core.Models.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public int? BillingInvoiceId { get; set; } 
        public BillingInvoice? BillingInvoice { get; set; }
    }
}
