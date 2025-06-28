using Core.Enums;

namespace Applications.Records.Schedule
{
    public record ScheduleItemRecord
    {
        public int ScheduleId { get; init; }
        public int ServiceOrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public string PatientName { get; init; } = string.Empty;
        public string ClientName { get; init; } = string.Empty;
        public DateTime ScheduledDate { get; init; }
        public ScheduledDeliveryType DeliveryType { get; init; }
        public bool IsOverdue { get; init; }
        public bool IsDelivered { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? CurrentSectorName { get; init; }
    }
}