namespace Applications.Dtos.Payments
{
    public class CreatePaymentDto
    {
        public int ClientId { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
         
    }
}
