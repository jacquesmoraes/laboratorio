namespace Applications.Dtos.Pricing
{
    public class UpdateTablePriceItemDto
    {
        public int Id { get; set; }
        public int WorkTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
