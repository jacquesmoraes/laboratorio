using Applications.Projections.ClientArea;

namespace Applications.Contracts
{
    public interface IBillingService : IGenericService<BillingInvoice>
    {
        Task<BillingInvoice> GenerateInvoiceAsync ( CreateBillingInvoiceDto dto );

        Task<BillingInvoice> CancelInvoiceAsync ( int id );

       Task<Pagination<ClientAreaInvoiceProjection>> GetPaginatedInvoicesForClientAreaAsync(InvoiceParams p);

        Task<BillingInvoiceRecord?> GetInvoiceRecordByIdAsync ( int id );
        Task<Pagination<BillingInvoiceResponseProjection>> GetPaginatedInvoicesAsync ( InvoiceParams p );

    }
}
