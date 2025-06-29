namespace Applications.Dtos.Schedule
{
    public class ScheduleDeliveryDto
    {
        public int ServiceOrderId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public ScheduledDeliveryType? DeliveryType { get; set; }

        public int? SectorId { get; set; }
    }
}
