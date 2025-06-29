using Core.Enums;
using Core.Models.ServiceOrders;

namespace Core.Models.Schedule
{
    public class ServiceOrderSchedule
    {
        public int Id { get; set; }

        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; } = null!;

        public int? SectorId { get; set; }
        public Sector? Sector { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public ScheduledDeliveryType? DeliveryType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDelivered { get; set; } = false;

        public bool IsOverdue { get; set; } = false;
    }
}
