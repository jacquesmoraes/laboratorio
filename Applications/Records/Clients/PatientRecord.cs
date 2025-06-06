namespace Applications.Records.Clients
{
    public record PatientRecord
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
}
