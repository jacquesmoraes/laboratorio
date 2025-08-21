namespace Applications.Records.Clients
{
    public record UpcomingDeliveriesRecord
    {
        public int ScheduleId { get; init; }
        public int ServiceOrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public string PatientName { get; init; } = string.Empty;
        public DateTime ScheduledDate { get; init; }
        public ScheduledDeliveryType DeliveryType { get; init; }
        public bool IsOverdue { get; init; }
    }
}
