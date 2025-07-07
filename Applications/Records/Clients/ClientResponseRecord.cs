namespace Applications.Records.Clients
{
    public record ClientResponseRecord
    {
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public string? ClientPhoneNumber { get; init; }
        public string? City { get; init; }
        public bool IsInactive { get; init; }
        public int BillingMode { get; init; }
        public string? TablePriceName { get; init; }
         public int? TablePriceId { get; init; }
    }
}
