namespace Applications.Dtos.Settings
{
    public class UpdateSystemSettingsDto
    {
        public string LabName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string FooterMessage { get; set; } = string.Empty;

        public LabAddressRecord LabAddressRecord { get; set; } = new LabAddressRecord
        { 
            Street = string.Empty,
            Cep = string.Empty,
            Number = 0,
            Complement = string.Empty,
            Neighborhood = string.Empty,
            City = string.Empty
        };
    }
}
