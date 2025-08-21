using System.Diagnostics;
using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaDashboardSpecification;
using static Core.FactorySpecifications.ClientAreaSpecifications.ClientAreaScheduleSpecification;

namespace Applications.Services.ClientAreaServices
{
    public class ClientAreaService (
        IGenericRepository<Client> clientRepo,
        IGenericRepository<BillingInvoice> invoiceRepo,
        IGenericRepository<Payment> paymentRepo,
        IGenericRepository<ServiceOrderSchedule> scheduleRepo,
        ILogger<ClientAreaService> logger
        ) : GenericService<Client>( clientRepo ), IClientAreaService
    {
        private readonly IGenericRepository<BillingInvoice> _invoiceRepo = invoiceRepo;
        private readonly IGenericRepository<Payment> _paymentRepo = paymentRepo;
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IGenericRepository<ServiceOrderSchedule> _scheduleRepo = scheduleRepo;
        private readonly ILogger<ClientAreaService> _logger = logger;

        public async Task<ClientDashboardRecord> GetClientBasicDashboardAsync ( int clientId )
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation ( "Starting dashboard generation for client {ClientId}", clientId );

            // 1. Buscar cliente com endereço
            var client = await _clientRepo.GetEntityWithSpecWithoutTrackingAsync(
                ClientAreaDashboardSpecs.ForDashboard(clientId))
                ?? throw new NotFoundException("Client not found.");

            _logger.LogDebug ( "Client query completed in {ElapsedMs}ms for client {ClientId}",
                stopwatch.ElapsedMilliseconds, clientId );

            // 2. Calcular total faturado
            var invoiceStopwatch = Stopwatch.StartNew();
            var totalInvoiced = await _invoiceRepo.SumAsync(
                i => i.ClientId == clientId && i.Status != InvoiceStatus.Cancelled,
                i => i.TotalServiceOrdersAmount);
            invoiceStopwatch.Stop ( );

            _logger.LogDebug ( "Invoice calculation completed in {ElapsedMs}ms for client {ClientId}",
                invoiceStopwatch.ElapsedMilliseconds, clientId );

            // 3. Calcular total pago
            var paymentStopwatch = Stopwatch.StartNew();
            var totalPaid = await _paymentRepo.SumAsync(
                p => p.ClientId == clientId,
                p => p.AmountPaid);
            paymentStopwatch.Stop ( );

            _logger.LogDebug ( "Payment calculation completed in {ElapsedMs}ms for client {ClientId}",
                paymentStopwatch.ElapsedMilliseconds, clientId );

            var balance = totalPaid - totalInvoiced;

            // 4. Buscar próximas entregas (agendamentos)
            var scheduleStopwatch = Stopwatch.StartNew();
            var scheduleSpec = ClientAreaScheduleSpecs.UpcomingDeliveriesByClient(clientId);
            var schedules = await _scheduleRepo.GetAllWithoutTrackingAsync(scheduleSpec);
            scheduleStopwatch.Stop ( );

            _logger.LogDebug ( "Schedule query completed in {ElapsedMs}ms for client {ClientId}",
                scheduleStopwatch.ElapsedMilliseconds, clientId );

            // 5. Mapear agendamentos para record de resposta
            var upcomingDeliveries = schedules.Select(s => new UpcomingDeliveriesRecord
            {
                ScheduleId = s.Id,
                ServiceOrderId = s.ServiceOrderId,
                OrderNumber = s.ServiceOrder.OrderNumber,
                PatientName = s.ServiceOrder.PatientName,
                ScheduledDate = s.ScheduledDate!.Value, // seguro por causa da spec
                DeliveryType = s.DeliveryType ?? ScheduledDeliveryType.FinalDelivery,
                IsOverdue = s.ScheduledDate < DateTime.Today && !s.IsDelivered
            }).ToList();

            stopwatch.Stop ( );
            _logger.LogInformation ( "Dashboard generation completed in {ElapsedMs}ms for client {ClientId}. " +
                "TotalInvoiced: {TotalInvoiced}, TotalPaid: {TotalPaid}, Balance: {Balance}",
                stopwatch.ElapsedMilliseconds, clientId, totalInvoiced, totalPaid, balance );

            // 6. Retornar dashboard completo
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
                Balance = balance,
                UpcomingDeliveries = upcomingDeliveries
            };
        }
    }
}
