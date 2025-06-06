using Applications.Records.Clients;
using Applications.Records.Production;
using Applications.Records.Work;

namespace Applications.Projections.ServiceOrder
{
   public record ServiceOrderDetailsProjection
{
    public int ServiceOrderId { get; init; }
    public int OrderNumber { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime DateIn { get; init; }
    public DateTime DateOut { get; init; }
    public DateTime? DateOutFinal { get; init; }
    public decimal OrderTotal { get; init; }
    public string? CurrentSectorName { get; set; }
    public ClientInvoiceRecord Client { get; init; } = new();
    public List<WorkRecord> Works { get; init; } = [];
    public List<StageRecord> Stages { get; init; } = [];
}
}
