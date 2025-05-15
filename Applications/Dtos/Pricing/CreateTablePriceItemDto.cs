namespace Applications.Dtos.Pricing
{
    public class CreateTablePriceItemDto
    {

        public int TablePriceId { get; set; }
        public int WorkTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
