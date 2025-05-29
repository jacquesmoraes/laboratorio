namespace Applications.Dtos.Clients
{
    public class ClientBalanceDto
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Balance => TotalPaid - TotalInvoiced;
    }

}
