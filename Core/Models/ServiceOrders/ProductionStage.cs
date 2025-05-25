namespace Core.Models.ServiceOrders
{
    public class ProductionStage
    {
        public int Id { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public int SectorId { get; set; }
        public required Sector Sector { get; set; }
        public int ServiceOrderId { get; set; }
        public required ServiceOrder ServiceOrder { get; set; }

    }
}