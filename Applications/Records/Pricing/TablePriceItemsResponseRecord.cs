namespace Applications.Records.Pricing
{
    public record TablePriceItemResponseRecord
    {
        public int Id { get; init; }
        public int WorkTypeId { get; init; }
        public string WorkTypeName { get; init; } = string.Empty;
        public decimal Price { get; init; }

        public int TablePriceId { get; init; }
        public string TablePriceName { get; init; } = string.Empty;
    }
}
