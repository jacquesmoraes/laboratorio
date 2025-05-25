using Core.Models.Shared;

namespace Core.Models.Clients
{
    public class PerClientPayment : PaymentBase
    {
        
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}
