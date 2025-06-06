namespace Applications.Records.Pricing
{
    public record TablePriceItemShortRecord
{
    public int Id { get; init; }
    public string WorkTypeName { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
}
