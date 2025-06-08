using Applications.Contracts;
using Core.Exceptions;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Core.Interfaces;
using Core.Models.Clients;

namespace Applications.Services
{
    public class ClientBalanceService ( IGenericRepository<Client> clientRepo ) : IClientBalanceService
    {
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;

        public async Task<ClientBalance> GetClientBalanceAsync ( int clientId )
        {
            var client = await _clientRepo.GetEntityWithSpec(
        ClientSpecification.ClientSpecs.ByIdFullForBalance(clientId));

            if ( client is null )
                throw new NotFoundException ( "Cliente não encontrado." );

            return ClientBalance.Calculate ( client );
        }

    }


}
