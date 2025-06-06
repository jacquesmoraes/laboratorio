namespace Applications.Records.Production
{
    public record StageRecord
    {
        public int SectorId { get; init; }
        public string SectorName { get; init; } = string.Empty;
        public DateTime DateIn { get; init; }
        public DateTime? DateOut { get; init; }
    }
}
