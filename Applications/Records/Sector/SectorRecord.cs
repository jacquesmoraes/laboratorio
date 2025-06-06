namespace Applications.Records.Sector
{
    public record SectorRecord
    {
        public int SectorId { get; init; }
        public string Name { get; init; } = string.Empty;
    }

}
