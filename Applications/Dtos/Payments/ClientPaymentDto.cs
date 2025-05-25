namespace Applications.Dtos.Payments
{
    public class ClientPaymentDto
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }

}
