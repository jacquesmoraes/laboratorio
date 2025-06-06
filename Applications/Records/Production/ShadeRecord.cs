namespace Applications.Records.Production
{
    public record ShadeRecord
    {
        public int Id { get; init; }
        public string? Color { get; init; }
        public int ScaleId { get; init; }
    }
}
