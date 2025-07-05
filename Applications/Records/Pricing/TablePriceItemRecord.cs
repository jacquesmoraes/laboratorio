namespace Applications.Records.Pricing
{
    public record TablePriceItemRecord
    {
        public int WorkTypeId { get; init; }
        public string WorkTypeName { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }
}
