using Core.Enums;
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
        public ClientBalance Balance { get; private set; } = new ( );
        public BillingMode BillingMode { get; set; }
        public string? Notes { get; set; }

        public Address Address { get; set; } = new ( );

        public int TablePriceId { get; set; }
        public TablePrice? TablePrice { get; set; }

        public List<Patient> Patients { get; set; } = [];
        public List<PerClientPayment> Payments { get; set; } = [];
        public List<ServiceOrder> ServiceOrders { get; set; } = [];


        public bool IsInactive =>
        ServiceOrders.OrderByDescending ( o => o.DateIn )
            .FirstOrDefault ( )?.DateIn < DateTime.Now.AddDays ( -60 );

        public void RegisterPayment ( decimal amount, DateTime paidAt )
        {
            var payment = new PerClientPayment
            {
                AmountPaid = amount,
                PaymentDate = paidAt,
                ClientId = this.ClientId
            };
            Payments.Add ( payment );
            Balance.AddCredit ( amount );
        }

        public void CloseMonth ( IEnumerable<ServiceOrder> finishedOrders )
        {
            foreach ( var so in finishedOrders )
            {
                if ( so.IsBillable )
                    Balance.AddDebtFromOrder ( so );
            }
        }
    }
}
