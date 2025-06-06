namespace Applications.Records.Clients
{
    public record ClientResponseForTablePriceRecord
    {
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public int BillingMode { get; init; }
        public string? TablePriceName { get; init; }
    }


}
