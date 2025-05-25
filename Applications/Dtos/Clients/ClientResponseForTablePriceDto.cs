namespace Applications.Dtos.Clients
{
    public class ClientResponseForTablePriceDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int BillingMode { get; set; }
        public string? TablePriceName { get; set; }
    }
}
