using Applications.Dtos.Payments;
using Applications.Dtos.ServiceOrder;

namespace Applications.Dtos.Clients
{
    public class ClientResponseDetailsDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string? ClientCpf { get; set; }
        public string? ClientPhoneNumber { get; set; }

        public int TablePriceId { get; set; }
        public string? TablePriceName { get; set; }

        public string? Street { get; set; }
        public int? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }

        public int BillingMode { get; set; }

        public bool IsInactive { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public ClientBalanceDto BalanceInfo { get; set; } = new();

        public List<ClientPaymentDto> Payments { get; set; } = [];
        public List<ServiceOrderListDto> ServiceOrders { get; set; } = [];

        
    }
}