namespace Core.Models.LabSettings
{
    public class SystemSettings
    {
        public int Id { get; set; } = 1;
        public string LabName { get; set; } = string.Empty;
        public LabAddress Address { get; set; } = new LabAddress ( );
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string FooterMessage { get; set; } = string.Empty;
        public string? LogoFileName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

}
