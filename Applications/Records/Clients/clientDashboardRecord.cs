namespace Applications.Records.Clients;

public record ClientDashboardRecord
{
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string? Street { get; init; }
    public int? Number { get; init; }
    public string? Complement { get; init; }
    public string? Neighborhood { get; init; }
    public string? City { get; init; }
    public string? PhoneNumber { get; init; }

    public List<MonthlyBalanceRecord> monthlyBalances { get; init; } = [];
    public List<UpcomingDeliveriesRecord> UpcomingDeliveries { get; init; } = [];
}
