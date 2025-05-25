using Applications.Dtos.Payments;
using Applications.Dtos.ServiceOrder;

namespace Applications.Dtos.Clients
{
    public class ClientResponseDto
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
        public List<PatientDto> Patients { get; set; } = [];
        public List<ClientPaymentDto> Payments { get; set; } = [];
        public List<ServiceOrderListDto> ServiceOrders { get; set; } = [];
    }
}

