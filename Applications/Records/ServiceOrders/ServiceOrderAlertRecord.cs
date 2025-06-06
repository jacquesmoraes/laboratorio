namespace Applications.Records.ServiceOrders
{
    public record ServiceOrderAlertRecord
    {
        public int ServiceOrderId { get; init; }
        public int OrderNumber { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public string ClientName { get; init; } = string.Empty;
        public string CurrentSectorName { get; set; } = string.Empty;
        public DateTime LastTryInDate { get; set; }
        public int DaysOut { get; set; }
        public string Status { get; init; } = string.Empty;
    }
}
