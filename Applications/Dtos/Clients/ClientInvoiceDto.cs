namespace Applications.Dtos.Clients
{
    public class ClientInvoiceDto
    {
        public string ClientName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
