namespace Applications.Projections.ServiceOrder
{
    public record ServiceOrderListProjection
    {
        public int ServiceOrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public DateTime DateIn { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? CurrentSectorName { get; set; }
    }
}
