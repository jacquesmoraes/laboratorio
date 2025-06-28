namespace Applications.Records.Schedule
{
    public record SectorScheduleRecord
    {
        public int SectorId { get; init; }
        public string SectorName { get; init; } = string.Empty;
        public List<ScheduleItemRecord> Deliveries { get; init; } = [];
    }
}