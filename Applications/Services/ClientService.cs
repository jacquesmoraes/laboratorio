using Applications.Contracts;
using Applications.Dtos.Clients;
using Core.Enums;
using Core.Interfaces;
using Core.Models.Clients;
using static Core.FactorySpecifications.ClientsFactorySpecifications.ClientSpecification;

namespace Applications.Services
{
    public class ClientService ( IGenericRepository<Client> repository ) : 
        GenericService<Client> ( repository ), IClientService
    {
        private readonly IGenericRepository<Client> _repository = repository;


        public async Task<Client?> GetClientIfEligibleForPerClientPayment ( int clientId )
        {
            var spec = ClientSpecs.ById(clientId);
            var client = await _repository.GetEntityWithSpec(spec);

            if ( client == null || client.BillingMode != BillingMode.perMonth )
                return null;

            return client;
        }




        public async Task<Client?> UpdateFromDtoAsync ( UpdateClientDto dto )
        {
            var spec = ClientSpecs.ById(dto.ClientId);
            var existing = await _repository.GetEntityWithSpec(spec);
            if ( existing == null )
                return null;

            // Atualiza os campos diretos
            existing.ClientName = dto.ClientName;
            existing.ClientEmail = dto.ClientEmail;
            existing.ClientPhoneNumber = dto.ClientPhoneNumber;
            existing.ClientCpf = dto.ClientCpf;
            existing.BillingMode = dto.BillingMode;
            existing.TablePriceId = dto.TablePriceId;

            // Atualiza o Address manualmente
            existing.Address.Street = dto.Address.Street;
            existing.Address.Number = dto.Address.Number;
            existing.Address.Complement = dto.Address.Complement;
            existing.Address.Neighborhood = dto.Address.Neighborhood;
            existing.Address.City = dto.Address.City;

            return await _repository.UpdateAsync ( dto.ClientId, existing );
        }
    }

}
