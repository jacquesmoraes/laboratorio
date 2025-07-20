namespace Applications.Projections.ClientArea;

public record ClientAreaServiceOrderProjection
{
    public int ServiceOrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public DateTime DateIn { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public decimal OrderTotal { get; init; }
    public OrderStatus Status { get; init; }
}
