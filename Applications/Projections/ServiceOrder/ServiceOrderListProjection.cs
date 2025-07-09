namespace Applications.Projections.ServiceOrder
{
    public record ServiceOrderListProjection
    {
        public int ServiceOrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public DateTime DateIn { get; init; }
        public DateTime? LastMovementDate { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
         public string ClientName { get; init; } = string.Empty;
         public int ClientId { get; init; }
        public string? CurrentSectorName { get; set; }
    }
}
