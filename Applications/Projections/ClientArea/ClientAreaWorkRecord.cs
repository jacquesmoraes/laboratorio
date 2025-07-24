namespace Applications.Projections.ClientArea
{
    public record ClientAreaWorkRecord
    {
        public int WorkTypeId { get; init; }
        public string WorkTypeName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal PriceUnit { get; init; }
        public string ShadeColor { get; init; } = string.Empty;
        public string ScaleName { get; init; } = string.Empty;
        public string? Notes { get; init; } = string.Empty;
    }
}
