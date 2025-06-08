using Applications.Contracts;
using Applications.Dtos.Clients;
using Core.Enums;
using Core.Exceptions;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.Pricing;
using static Core.FactorySpecifications.ClientsFactorySpecifications.ClientSpecification;

namespace Applications.Services
{
    public class ClientService ( IGenericRepository<Client> repository, IGenericRepository<TablePrice> tablePriceRepository ) :
        GenericService<Client> ( repository ), IClientService
    {
        private readonly IGenericRepository<Client> _repository = repository;
        private readonly IGenericRepository<TablePrice> _tablePriceRepository = tablePriceRepository;
        public async Task<Client> CreateClientAsync ( Client entity )
        {
            // Input validation
            if ( string.IsNullOrWhiteSpace ( entity.ClientName ) )
                throw new CustomValidationException ( "O nome do cliente é obrigatório." );

            if ( entity.TablePriceId <= 0 )
                throw new CustomValidationException ( "É necessário informar uma Tabela de Preço válida." );

            // Validate table price if provided
             if ( entity.TablePriceId.HasValue && entity.TablePriceId.Value > 0 )
            {
                var tablePrice = await _tablePriceRepository.GetEntityWithSpec(
                    TablePriceSpecs.ById(entity.TablePriceId.Value)
                    ) ?? throw new NotFoundException($"Tabela de preço com ID {entity.TablePriceId} não encontrada.");
                if ( !tablePrice.Status )
                    throw new BusinessRuleException ( "Não é possível associar o cliente a uma tabela de preço inativa." );
            }


            return await base.CreateAsync ( entity );
        }

        public async Task<Client?> GetClientIfEligibleForPerClientPayment ( int clientId )
        {
            if ( clientId <= 0 )
                throw new CustomValidationException ( "ID do cliente inválido." );

            var spec = ClientSpecs.ById(clientId);
            var client = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Cliente com ID {clientId} não encontrado.");

            if ( client.BillingMode != BillingMode.perMonth )
                throw new BusinessRuleException ( "Apenas clientes com cobrança mensal são elegíveis para pagamento por cliente." );

            return client;
        }




        public async Task<Client?> UpdateFromDtoAsync ( UpdateClientDto dto )
        {
            if ( dto.ClientId <= 0 )
                throw new CustomValidationException ( "ID do cliente inválido." );

            if ( string.IsNullOrWhiteSpace ( dto.ClientName ) )
                throw new CustomValidationException ( "O nome do cliente é obrigatório." );

            var spec = ClientSpecs.ById(dto.ClientId);
            var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Cliente com ID {dto.ClientId} não encontrado.");
            if ( dto.BillingMode != existing.BillingMode )
            {
                // Verifica se existem ordens de serviço em aberto
                if ( existing.ServiceOrders.Any ( o => o.Status != OrderStatus.Finished ) )
                    throw new BusinessRuleException ( "Não é possível alterar o modo de cobrança enquanto houver ordens de serviço em aberto." );
            }

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

            var updated = await _repository.UpdateAsync(dto.ClientId, existing)
                ?? throw new BusinessRuleException("Falha ao atualizar o cliente.");

            return updated;
        }
    }

}
