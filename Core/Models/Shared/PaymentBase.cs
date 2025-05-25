namespace Core.Models.Shared
{
    public  class PaymentBase
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string? Description { get; set; }
    }
}
