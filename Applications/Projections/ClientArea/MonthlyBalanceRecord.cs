namespace Applications.Projections.ClientArea
{
    public record MonthlyBalanceRecord
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public string MonthName { get; init; } = string.Empty;
        public decimal Invoiced { get; init; }
        public decimal Paid { get; init; }
        public decimal Balance { get; init; }
        public bool IsCurrentMonth { get; init; }
    }
}