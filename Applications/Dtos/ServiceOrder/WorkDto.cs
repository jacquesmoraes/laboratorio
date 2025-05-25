namespace Applications.Dtos.ServiceOrder
{
    public class WorkDto
    {
        public int WorkTypeId { get; set; }
        public string WorkTypeName { get; set; } = string.Empty; // do WorkType.Name
        public int Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal PriceTotal => Quantity * PriceUnit;
        public string? ShadeColor { get; set; }
        public string? ScaleName { get; set; }
        public string? Notes { get; set; }
    }
}
