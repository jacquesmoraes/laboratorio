using Core.Models.ServiceOrders;

namespace Core.Models.Works
{
    public class Work
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal PriceTotal => Quantity * PriceUnit;
        public bool IsActive { get; set; }



        public int WorkTypeId { get; set; }
        public WorkType WorkType { get; set; } = null!;

        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; } = null!;

    }
}
