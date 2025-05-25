using Core.Enums;

namespace Applications.Dtos.Clients
{
    public class UpdateClientDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string? ClientPhoneNumber { get; set; }
        public string? ClientCpf { get; set; }
        public BillingMode BillingMode { get; set; }
        public int TablePriceId { get; set; }
        public AddressDto Address { get; set; } = new ( );
    }
}
