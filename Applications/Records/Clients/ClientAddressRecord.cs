namespace Applications.Records.Clients
{
    public record ClientAddressRecord
    {
        public string Street { get; init; } = string.Empty;
        public int Number { get; init; }
        public string Complement { get; init; } = string.Empty;
        public string Neighborhood { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
    }

}
