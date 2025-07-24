namespace Applications.Projections.ClientArea;

public record ClientAreaServiceOrderDetailsProjection
{
    public int ServiceOrderId { get; init; }
    public string OrderNumber { get; init; } = default!;
    public DateTime DateIn { get; init; }
    public DateTime? DateOut { get; init; }
    public string PatientName { get; init; } = default!;
    public string Status { get; init; } = default!;
    public decimal OrderTotal { get; init; }

    public IReadOnlyList<ClientAreaWorkRecord> Works { get; init; } = [];
    public IReadOnlyList<StageRecord> Stages { get; init; } = [];
}




