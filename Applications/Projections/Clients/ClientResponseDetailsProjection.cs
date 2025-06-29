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
        public decimal TotalPaid { get; init; }
        public decimal TotalInvoiced { get; init; }
        public decimal Balance => TotalPaid - TotalInvoiced;
        public ClientAddressRecord Address { get; init; } = new ClientAddressRecord ( );
        public List<ServiceOrderShortRecord> ServiceOrders { get; init; } = [];
        public List<ClientPaymentRecord> Payments { get; init; } = [];
    }

}
