namespace Applications.Records.Clients
{
    public record ClientInvoiceRecord
    {
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public ClientAddressRecord Address { get; init; } = new();
        public string? PhoneNumber { get; init; }
    }
}
