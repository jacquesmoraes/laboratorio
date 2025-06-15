namespace Applications.Records.Clients
{
    public record ClientInvoiceRecord
    {
        public string ClientName { get; init; } = string.Empty;
        public ClientAddressRecord Address { get; init; } = new();
        public string? PhoneNumber { get; init; }
    }
}
