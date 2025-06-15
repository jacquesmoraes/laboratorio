namespace Applications.Contracts.Pdf
{
    public interface IBillingInvoicePdfService
    {
        Task<byte[]> GenerateInvoicePdfAsync(int invoiceId);
    }
}
