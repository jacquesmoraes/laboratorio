namespace Applications.Records.Clients
{
    public record ClientInvoiceRecord
    {
        public string ClientName { get; init; } = string.Empty;
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
    }
}
