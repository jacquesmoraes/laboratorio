namespace Applications.Dtos.ServiceOrder
{
    public class CreateWorkDto
    {
        public int WorkTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceUnit { get; set; }

        public int? ShadeId { get; set; }      
        public int? ScaleId { get; set; }      
      
        public string? Notes { get; set; }
    }

}
