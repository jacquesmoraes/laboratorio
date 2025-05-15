using Core.Enums;

namespace Core.Models.ServiceOrders
{
    public class ProductionStage
    {
        public int Id { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public ProductionStep Step { get; set; }

    }
}