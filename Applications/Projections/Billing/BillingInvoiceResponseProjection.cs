using Applications.Records.Billing;
using Applications.Records.Clients;
using Core.Enums;

namespace Applications.Projections.Billing;

public record BillingInvoiceResponseProjection
{
    public int BillingInvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }

    public ClientInvoiceRecord Client { get; init; } = new();

    public List<InvoiceServiceOrderRecord> ServiceOrders { get; init; } = [];
   
    public InvoiceStatus Status { get; init; }

    public decimal TotalServiceOrdersAmount { get; init; }

    public decimal PreviousCredit { get; init; }
    public decimal PreviousDebit { get; init; }

    public decimal TotalInvoiceAmount { get; init; }

    public decimal TotalPaid { get; init; }

    public decimal OutstandingBalance => TotalInvoiceAmount - TotalPaid;
}
