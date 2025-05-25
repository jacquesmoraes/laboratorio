namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderListDto
    {
        public int ServiceOrderId { get; set; }
        public int OrderNumber { get; set; }
        public DateTime DateIn { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CurrentSectorName { get; set; }
    }

}
