namespace Applications.Records.Pricing
{
    public record TablePriceItemsResponseRecord
    {
        public int Id { get; init; }
        public string? TablePriceName { get; init; }
        public string? WorkTypeName { get; init; }
        public decimal Price { get; init; }
    }
}
