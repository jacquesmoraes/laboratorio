namespace Applications.Dtos.Pricing
{
    public class TablePriceItemsResponseDto
    {
        public int Id { get; set; }
        public string? TablePriceName { get; set; }
        public string? WorkTypeName { get; set; }
        public decimal Price { get; set; }
    }
}
