using Core.Enums;
using Core.Models.Billing;
using Core.Models.Payments;
using Core.Models.Pricing;
using Core.Models.ServiceOrders;

namespace Core.Models.Clients
{
    public class Client
    {
        public int ClientId { get; set; }
        public required string ClientName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ClientEmail { get; set; }
        public string? Cro { get; set; }
        public string? Cnpj { get; set; }
        public string? ClientCpf { get; set; }
        public string? ClientPhoneNumber { get; set; }

        public BillingMode BillingMode { get; set; }
        public string? Notes { get; set; }

        public Address Address { get; set; } = new ( );

        public int TablePriceId { get; set; }
        public TablePrice? TablePrice { get; set; }

        public List<Patient> Patients { get; set; } = [];
        public List<Payment> Payments { get; set; } = [];
        public List<ServiceOrder> ServiceOrders { get; set; } = [];
         public List<BillingInvoice> BillingInvoices { get; set; } = [];
        public bool IsInactive =>
            ServiceOrders.OrderByDescending ( o => o.DateIn )
                         .FirstOrDefault ( )?.DateIn < DateTime.Now.AddDays ( -60 );
    }
}
