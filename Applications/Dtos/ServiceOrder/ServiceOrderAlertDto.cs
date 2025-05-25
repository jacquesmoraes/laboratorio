namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderAlertDto
    {
        public int ServiceOrderId { get; set; }
        public int OrderNumber { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string CurrentSectorName { get; set; } = string.Empty;
        public DateTime LastTryInDate { get; set; }
        public int DaysOut { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
