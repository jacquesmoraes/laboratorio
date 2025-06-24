using Applications.Dtos.Payments;
using Applications.Records.Payments;
using Applications.Responses;
using Core.Models.Payments;
using Core.Params;

namespace Applications.Contracts
{
    public interface IPaymentService : IGenericService<Payment> {

         Task<Payment> RegisterClientPaymentAsync(CreatePaymentDto dto);
        Task<Pagination<ClientPaymentRecord>> GetPaginatedAsync(PaymentParams p);

    }

    
}
