namespace Applications.Records.Schedule
{
    public record DailyScheduleRecord
    {
        public DateTime Date { get; init; }
        public List<SectorScheduleRecord> Sectors { get; init; } = [];
        public int TotalDeliveries { get; init; }
        public int OverdueDeliveries { get; init; }
    }
}