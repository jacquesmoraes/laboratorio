namespace Applications.Dtos.ServiceOrder
{
    public class MoveToStageDto
    {
        public int ServiceOrderId { get; set; }
        public int SectorId { get; set; }
        public DateTime DateIn { get; set; }
    }
}
