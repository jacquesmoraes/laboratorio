using Applications.Contracts;
using Core.Interfaces;
using Core.Models.Clients;

namespace Applications.Services
{
    public class ClientPaymentService ( IGenericRepository<PerClientPayment> repository ) : GenericService<PerClientPayment> ( repository ), IClientPaymentService
    {
    }


}
