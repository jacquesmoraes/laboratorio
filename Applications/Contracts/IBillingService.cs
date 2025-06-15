using Applications.Dtos.Billing;
using Applications.Records.Billing;
using Core.Models.Billing;

namespace Applications.Contracts
{
    public interface IBillingService : IGenericService<BillingInvoice>
    {
        Task<BillingInvoice> GenerateInvoiceAsync(CreateBillingInvoiceDto dto);

        Task<BillingInvoice> CancelInvoiceAsync(int id);

        /// <summary>
        /// Retorna o record de leitura completo para geração de PDF ou visualização.
        /// </summary>
        Task<BillingInvoiceRecord?> GetInvoiceRecordByIdAsync(int id);
    }
}
