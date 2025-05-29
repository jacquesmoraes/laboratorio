using Applications.Dtos.Payments;
using Applications.Dtos.ServiceOrder;

namespace Applications.Dtos.Clients
{
    public class ClientResponseDto
    {
       public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? ClientPhoneNumber { get; set; }
    public string? City { get; set; }
    public bool IsInactive { get; set; }
    }
}

