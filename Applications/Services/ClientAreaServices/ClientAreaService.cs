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
        ) : GenericService<Client> ( clientRepo ), IClientAreaService
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

            // 1. get client basic info
            var client = await _clientRepo.GetEntityWithSpecWithoutTrackingAsync(
                ClientAreaDashboardSpecs.ForDashboard(clientId))
                ?? throw new NotFoundException("Client not found.");

            _logger.LogDebug ( "Client query completed in {ElapsedMs}ms for client {ClientId}",
                stopwatch.ElapsedMilliseconds, clientId );

            // 2. calculate current balance
            var balanceStopwatch = Stopwatch.StartNew();
            var totalInvoiced = await _invoiceRepo.SumAsync(
                i => i.ClientId == clientId && i.Status != InvoiceStatus.Cancelled,
                i => i.TotalServiceOrdersAmount);

            var totalPaid = await _paymentRepo.SumAsync(
                p => p.ClientId == clientId,
                p => p.AmountPaid);

            var currentBalance = totalPaid - totalInvoiced;
            balanceStopwatch.Stop ( );

            _logger.LogDebug ( "Balance calculation completed in {ElapsedMs}ms for client {ClientId}",
                balanceStopwatch.ElapsedMilliseconds, clientId );

            // 3. calculate monthly balances for last 12 months
            var monthlyBalanceStopwatch = Stopwatch.StartNew();
            var monthlyBalances = await GetMonthlyBalancesAsync(clientId, 12);
            monthlyBalanceStopwatch.Stop ( );

            _logger.LogDebug ( "Monthly balances calculation completed in {ElapsedMs}ms for client {ClientId}",
                monthlyBalanceStopwatch.ElapsedMilliseconds, clientId );

            // 4. load upcoming deliveries (next 5)
            var scheduleStopwatch = Stopwatch.StartNew();
            var scheduleSpec = ClientAreaScheduleSpecs.UpcomingDeliveriesByClient(clientId);
            var schedules = await _scheduleRepo.GetAllWithoutTrackingAsync(scheduleSpec);
            scheduleStopwatch.Stop ( );

            _logger.LogDebug ( "Schedule query completed in {ElapsedMs}ms for client {ClientId}",
                scheduleStopwatch.ElapsedMilliseconds, clientId );

            // 5. map to upcoming deliveries record
            var upcomingDeliveries = schedules.Select(s => new UpcomingDeliveriesRecord
            {
                ScheduleId = s.Id,
                ServiceOrderId = s.ServiceOrderId,
                OrderNumber = s.ServiceOrder.OrderNumber,
                PatientName = s.ServiceOrder.PatientName,
                ScheduledDate = s.ScheduledDate!.Value,
                DeliveryType = s.DeliveryType ?? ScheduledDeliveryType.FinalDelivery,
                IsOverdue = s.ScheduledDate < DateTime.Today && !s.IsDelivered
            }).ToList();

            stopwatch.Stop ( );
            _logger.LogInformation ( "Dashboard generation completed in {ElapsedMs}ms for client {ClientId}. " +
                "CurrentBalance: {CurrentBalance}, MonthlyBalances: {MonthlyBalancesCount}",
                stopwatch.ElapsedMilliseconds, clientId, currentBalance, monthlyBalances.Count );

            // 6. return complete dashboard record
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
                monthlyBalances = monthlyBalances,
                UpcomingDeliveries = upcomingDeliveries
            };
        }

        private async Task<List<MonthlyBalanceRecord>> GetMonthlyBalancesAsync ( int clientId, int months = 12 )
        {
            var totalStopwatch = Stopwatch.StartNew();
            _logger.LogDebug ( "Starting monthly balances calculation for client {ClientId}, months: {Months}", clientId, months );

            var monthlyBalances = new List<MonthlyBalanceRecord>();
            var currentDate = DateTime.Today;

            // Primeiro dia do mês mais antigo da janela
            var startOfWindow = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-(months - 1));

            // Saldo de abertura (antes da janela)
            var openingInvoiceSum = await _invoiceRepo.SumAsync(
                i => i.ClientId == clientId && i.Status != InvoiceStatus.Cancelled && i.CreatedAt < startOfWindow,
                i => i.TotalServiceOrdersAmount);
            var openingPaymentSum = await _paymentRepo.SumAsync(
                p => p.ClientId == clientId && p.PaymentDate < startOfWindow,
                p => p.AmountPaid);

            var runningBalance = openingPaymentSum - openingInvoiceSum; // negativo = dívida, positivo = crédito

            // Iterar do mais antigo até o mês atual
            for ( int j = 0; j < months; j++ )
            {
                var targetDate = startOfWindow.AddMonths(j);
                var startOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var monthlyInvoiced = await _invoiceRepo.SumAsync(
        i => i.ClientId == clientId &&
             i.Status != InvoiceStatus.Cancelled &&
             i.CreatedAt >= startOfMonth &&
             i.CreatedAt <= endOfMonth,
        i => i.TotalServiceOrdersAmount);

                var monthlyPaid = await _paymentRepo.SumAsync(
        p => p.ClientId == clientId &&
             p.PaymentDate >= startOfMonth &&
             p.PaymentDate <= endOfMonth,
        p => p.AmountPaid);

                runningBalance += ( monthlyPaid - monthlyInvoiced );

                monthlyBalances.Add ( new MonthlyBalanceRecord
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    MonthName = targetDate.ToString ( "MMMM yyyy", new CultureInfo ( "pt-BR" ) ),
                    Invoiced = monthlyInvoiced,
                    Paid = monthlyPaid,
                    Balance = runningBalance,
                    IsCurrentMonth = targetDate.Year == currentDate.Year && targetDate.Month == currentDate.Month
                } );
            }

            // Deixar mês atual primeiro (como já era)
            monthlyBalances.Reverse ( );

            totalStopwatch.Stop ( );
            _logger.LogInformation ( "Monthly balances calculation completed in {ElapsedMs}ms for client {ClientId}. Generated {Count} monthly records",
                totalStopwatch.ElapsedMilliseconds, clientId, monthlyBalances.Count );

            return monthlyBalances;

        }
    }
}