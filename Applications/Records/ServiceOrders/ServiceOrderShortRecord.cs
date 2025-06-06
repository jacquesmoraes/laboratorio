namespace Applications.Records.ServiceOrders
{
    public record ServiceOrderShortRecord
    {
        public int ServiceOrderId { get; init; }
        public DateTime DateIn { get; init; }
        public decimal OrderTotal { get; init; }
    }
}
