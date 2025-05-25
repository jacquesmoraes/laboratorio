using Applications.Dtos.Clients;
using Core.Models.Clients;

namespace Applications.Contracts
{
    public interface IClientService : IGenericService<Client>
    {
         Task<Client?> GetClientIfEligibleForPerClientPayment ( int clientId );
        Task<Client?> UpdateFromDtoAsync(UpdateClientDto dto);
    }
    
}
