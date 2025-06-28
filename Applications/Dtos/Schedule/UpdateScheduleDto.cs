using Core.Enums;

namespace Applications.Dtos.Schedule
{
    public class UpdateScheduleDto
    {
        public DateTime ScheduledDate { get; set; }
        public ScheduledDeliveryType DeliveryType { get; set; }
        public int? SectorId { get; set; }
    }
}