namespace Applications.Records.Billing
{
    public record InvoiceWorkItemRecord
{
    public string WorkTypeName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal PriceUnit { get; init; }

    public decimal PriceTotal => Quantity * PriceUnit;
}
}
