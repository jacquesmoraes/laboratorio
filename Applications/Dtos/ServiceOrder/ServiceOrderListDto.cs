namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderListDto
    {
        public int ServiceOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public required string ClientName { get; init; } 
        public DateTime DateIn { get; set; }
        public int ClientId { get; set; }
        public DateTime? LastMovement { get; init; }
        public string PatientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CurrentSectorName { get; set; }
        public decimal OrderTotal { get; set; }
    }

}
