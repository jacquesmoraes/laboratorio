using Applications.Records.Clients;
using Applications.Records.Payments;
using Applications.Records.ServiceOrders;

namespace Applications.Projections.Clients
{
    public record ClientResponseDetailsProjection
    {
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public string? ClientPhoneNumber { get; init; }
        public string? City { get; init; }
        public bool IsInactive { get; init; }
         public int BillingMode { get; init; }
        public string? TablePriceName { get; init; }
        public ClientAddressRecord Address { get; init; } = new ClientAddressRecord();
        public List<ServiceOrderShortRecord> ServiceOrders { get; init; } = [];
        public List<ClientPaymentRecord> Payments { get; init; } = [];
    }

}
