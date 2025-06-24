using Applications.Dtos.Clients;
using Applications.Projections.Clients;
using Core.Models.Clients;

namespace Applications.Contracts
{
    public interface IClientService : IGenericService<Client>
    {
        Task<Client> CreateClientAsync(Client entity);
         Task<Client?> GetClientIfEligibleForPerClientPayment ( int clientId );
        Task<Client?> UpdateFromDtoAsync(UpdateClientDto dto);
        Task<ClientResponseDetailsProjection> GetClientDetailsProjectionAsync(int id);
    }
    
}
