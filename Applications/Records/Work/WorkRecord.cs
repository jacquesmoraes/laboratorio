namespace Applications.Records.Work
{
    public record WorkRecord
    {
        public int WorkTypeId { get; init; }
        public string WorkTypeName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal PriceUnit { get; init; }
        public decimal PriceTotal => Quantity * PriceUnit;
        public int? ShadeId { get; init; }
        public string? ShadeColor { get; init; }
        public int? ScaleId { get; init; }
        public string? ScaleName { get; init; }
        public string? Notes { get; init; }
    }

}
