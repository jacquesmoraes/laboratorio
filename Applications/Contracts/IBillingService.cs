using Applications.Dtos.Billing;
using Core.Models.Billing;

namespace Applications.Contracts
{
    public interface IBillingService : IGenericService<BillingInvoice>
    {
        Task<BillingInvoice> GenerateInvoiceAsync(CreateBillingInvoiceDto dto);
        
        Task<BillingInvoice> CancelInvoiceAsync ( int id );


    }
}
