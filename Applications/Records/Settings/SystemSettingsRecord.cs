namespace Applications.Records.Settings
{
    public record SystemSettingsRecord
    {
        public string LabName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public LabAddressRecord Address { get; init; } = default!;
        public string? LogoUrl { get; init; }
        public string Phone { get; init; } = string.Empty;
        public string CNPJ { get; init; } = string.Empty;
        public string FooterMessage { get; init; } = string.Empty;
        public DateTime LastUpdated { get; init; }

    }
}
