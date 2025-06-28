using Core.Enums;

namespace Applications.Records.Schedule
{
    public record ScheduledDeliveryRecord
    {
        public int ServiceOrderId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public string SectorName { get; init; } = string.Empty;
        public DateTime ScheduledDate { get; init; }
        public ScheduledDeliveryType DeliveryType { get; init; }
        public bool IsOverdue { get; init; }
    }
}