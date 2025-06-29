namespace Applications.Contracts
{
    public interface IBillingService : IGenericService<BillingInvoice>
    {
        Task<BillingInvoice> GenerateInvoiceAsync ( CreateBillingInvoiceDto dto );

        Task<BillingInvoice> CancelInvoiceAsync ( int id );

         /// <summary>
        /// Returns the complete read record for PDF generation or viewing.
        /// </summary>
        Task<BillingInvoiceRecord?> GetInvoiceRecordByIdAsync ( int id );
        Task<Pagination<BillingInvoiceResponseProjection>> GetPaginatedInvoicesAsync ( InvoiceParams p );

    }
}
