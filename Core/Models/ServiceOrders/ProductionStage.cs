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


          public static ProductionStage Create ( ServiceOrder order, Sector sector, DateTime dateIn )
        {
            return new ProductionStage
            {
                Sector = sector,
                SectorId = sector.SectorId,
                ServiceOrder = order,
                DateIn = dateIn
            };
        }
    }
}