namespace Applications.Dtos.Pricing
{
    public class CreateTablePriceItemDto
    {
        public required string ItemName { get; set; }
        public required decimal Price { get; set; }
    }
}
