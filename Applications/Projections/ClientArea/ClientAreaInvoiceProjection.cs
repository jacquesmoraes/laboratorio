namespace Applications.Projections.ClientArea;

public record ClientAreaInvoiceProjection
{
    public int BillingInvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public InvoiceStatus Status { get; init; }
    public decimal TotalInvoiceAmount { get; init; }
}