using System.Diagnostics;
using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaDashboardSpecification;

namespace Applications.Services
{
    public class ClientAreaService : GenericService<Client>, IClientAreaService
    {
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly ILogger<ClientAreaService> _logger;

        public ClientAreaService (
            IGenericRepository<Client> clientRepo,
            IGenericRepository<BillingInvoice> invoiceRepo,
            IGenericRepository<Payment> paymentRepo,
            ILogger<ClientAreaService> logger
        ) : base ( clientRepo )
        {
            _clientRepo = clientRepo;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _logger = logger;
        }

        public async Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId )
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation ( "Starting dashboard generation for client {ClientId}", clientId );

            // 1. Client query otimizada
            var client = await _clientRepo.GetEntityWithSpecWithoutTrackingAsync(
        ClientAreaDashboardSpecs.ForDashboard(clientId))
        ?? throw new NotFoundException("Client not found.");

            var clientQueryTime = stopwatch.ElapsedMilliseconds;
            _logger.LogDebug ( "Client query completed in {ElapsedMs}ms for client {ClientId}", clientQueryTime, clientId );

            // 2. Cálculos sequenciais (não em paralelo)
            var invoiceStopwatch = Stopwatch.StartNew();
            var totalInvoiced = await _invoiceRepo.SumAsync(
        i => i.ClientId == clientId && i.Status != InvoiceStatus.Cancelled,
        i => i.TotalServiceOrdersAmount);
            invoiceStopwatch.Stop ( );

            _logger.LogDebug ( "Invoice calculation completed in {ElapsedMs}ms for client {ClientId}",
                invoiceStopwatch.ElapsedMilliseconds, clientId );

            var paymentStopwatch = Stopwatch.StartNew();
            var totalPaid = await _paymentRepo.SumAsync(
        p => p.ClientId == clientId,
        p => p.AmountPaid);
            paymentStopwatch.Stop ( );

            _logger.LogDebug ( "Payment calculation completed in {ElapsedMs}ms for client {ClientId}",
                paymentStopwatch.ElapsedMilliseconds, clientId );

            var balance = totalPaid - totalInvoiced;

            stopwatch.Stop ( );
            _logger.LogInformation ( "Dashboard generation completed in {ElapsedMs}ms for client {ClientId}. " +
                "TotalInvoiced: {TotalInvoiced}, TotalPaid: {TotalPaid}, Balance: {Balance}",
                stopwatch.ElapsedMilliseconds, clientId, totalInvoiced, totalPaid, balance );

            return new ClientDashboardRecord
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Street = client.Address.Street,
                Number = client.Address.Number,
                Complement = client.Address.Complement,
                Neighborhood = client.Address.Neighborhood,
                City = client.Address.City,
                PhoneNumber = client.ClientPhoneNumber,
                TotalInvoiced = totalInvoiced,
                TotalPaid = totalPaid,
                Balance = balance
            };
        }

    }
}