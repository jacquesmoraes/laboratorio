namespace Applications.Dtos.Schedule
{
    public class CreateScheduleDto
    {
        public int ServiceOrderId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public ScheduledDeliveryType DeliveryType { get; set; }
        public int? SectorId { get; set; }
    }
}