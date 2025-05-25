namespace Applications.Dtos.Payments
{
    public class CreatePerClientPaymentDto
    {
        public int ClientId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }
    }
}
