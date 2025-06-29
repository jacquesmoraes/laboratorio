namespace Applications.Services
{
    public class ClientService (
        IGenericRepository<Client> clientRepository,
        IGenericRepository<TablePrice> tablePriceRepository,
        IGenericRepository<Payment> paymentRepository,
        IGenericRepository<BillingInvoice> invoiceRepository,
        IMapper mapper
    ) : GenericService<Client> ( clientRepository ), IClientService
    {
        private readonly IGenericRepository<Client> _clientRepo = clientRepository;
        private readonly IGenericRepository<TablePrice> _tablePriceRepository = tablePriceRepository;
        private readonly IGenericRepository<Payment> _paymentRepository = paymentRepository;
        private readonly IGenericRepository<BillingInvoice> _invoiceRepository = invoiceRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Client> CreateClientAsync ( Client entity )
        {
            // Input validation
            if ( string.IsNullOrWhiteSpace ( entity.ClientName ) )
                throw new CustomValidationException ( "Client name is required." );

            if ( entity.TablePriceId <= 0 )
                throw new CustomValidationException ( "A valid price table must be provided." );

            // Validate table price if provided
            if ( entity.TablePriceId.HasValue && entity.TablePriceId.Value > 0 )
            {
                var tablePrice = await _tablePriceRepository.GetEntityWithSpec(
                    TablePriceSpecs.ById(entity.TablePriceId.Value)
                ) ?? throw new NotFoundException($"Price table with ID {entity.TablePriceId} not found.");

                if ( !tablePrice.Status )
                    throw new BusinessRuleException ( "Cannot associate the client with an inactive price table." );
            }

            return await base.CreateAsync ( entity );
        }

        public async Task<Client?> GetClientIfEligibleForPerClientPayment ( int clientId )
        {
            if ( clientId <= 0 )
                throw new CustomValidationException ( "Invalid client ID." );

            var spec = ClientSpecs.ById(clientId);
            var client = await _clientRepo.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Client with ID {clientId} not found.");

            if ( client.BillingMode != BillingMode.perMonth )
                throw new BusinessRuleException ( "Only clients with monthly billing are eligible for per-client payments." );

            return client;
        }

        public async Task<ClientResponseDetailsProjection> GetClientDetailsProjectionAsync ( int clientId )
        {
            var spec = ClientSpecs.ById(clientId);

            var client = await _clientRepo.GetEntityWithSpec(spec)
                ?? throw new NotFoundException("Client not found.");

            // Consistent calculations
            var totalPaid = await _paymentRepository.SumAsync(
                p => p.ClientId == clientId,
                p => p.AmountPaid
            );

            var totalInvoiced = await _invoiceRepository.SumAsync(
                i => i.ClientId == clientId && i.Status != InvoiceStatus.Cancelled,
                i => i.TotalServiceOrdersAmount
            );

            // Map and manually set financial fields
            var projection = _mapper.Map<ClientResponseDetailsProjection>(client);
            projection = projection with
            {
                TotalPaid = totalPaid,
                TotalInvoiced = totalInvoiced
            };

            return projection;
        }

        public async Task<Client?> UpdateFromDtoAsync ( UpdateClientDto dto )
        {
            if ( dto.ClientId <= 0 )
                throw new CustomValidationException ( "Invalid client ID." );

            if ( string.IsNullOrWhiteSpace ( dto.ClientName ) )
                throw new CustomValidationException ( "Client name is required." );

            var spec = ClientSpecs.ById(dto.ClientId);
            var existing = await _clientRepo.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Client with ID {dto.ClientId} not found.");

            if ( dto.BillingMode != existing.BillingMode )
            {
                // Check for open service orders
                if ( existing.ServiceOrders.Any ( o => o.Status != OrderStatus.Finished ) )
                    throw new BusinessRuleException ( "Cannot change billing mode while there are open service orders." );
            }

            // Update direct fields
            existing.ClientName = dto.ClientName;
            existing.ClientEmail = dto.ClientEmail;
            existing.ClientPhoneNumber = dto.ClientPhoneNumber;
            existing.ClientCpf = dto.ClientCpf;
            existing.BillingMode = dto.BillingMode;
            existing.TablePriceId = dto.TablePriceId;

            // Update address fields manually
            existing.Address.Street = dto.Address.Street;
            existing.Address.Number = dto.Address.Number;
            existing.Address.Complement = dto.Address.Complement;
            existing.Address.Neighborhood = dto.Address.Neighborhood;
            existing.Address.City = dto.Address.City;

            var updated = await _clientRepo.UpdateAsync(dto.ClientId, existing)
                ?? throw new BusinessRuleException("Failed to update client.");

            return updated;
        }
    }
}
