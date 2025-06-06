namespace Applications.Records.Clients
{
   public record ClientBalanceRecord
{
    public decimal TotalPaid { get; init; }
    public decimal TotalInvoiced { get; init; }
    public decimal Credit { get; init; }
    public decimal Debit { get; init; }

    public decimal Balance => TotalPaid - TotalInvoiced;
}

}
