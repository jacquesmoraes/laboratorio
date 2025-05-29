using Applications.Dtos.Payments;
using Core.Models.Payments;

namespace Applications.Contracts
{
    public interface IPaymentService : IGenericService<Payment> {

         Task<Payment> RegisterClientPaymentAsync(CreatePaymentDto dto);
    }

    
}
