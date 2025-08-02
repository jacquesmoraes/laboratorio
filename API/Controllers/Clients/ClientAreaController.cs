using System.Diagnostics;

namespace API.Controllers.Clients
{
    [ApiController]
     [Authorize(Roles = "client")]
    [Route ( "api/client-area" )]
    public class ClientAreaController (
        IClientAreaService clientAreaService,
        IPaymentService paymentService,
        IBillingService billingService,
        IServiceOrderService serviceOrderService,
        ILogger<ClientAreaController> logger
        
    ) : ControllerBase
    {
        private readonly IClientAreaService _clientAreaService = clientAreaService;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IBillingService _billingService = billingService;
        private readonly IServiceOrderService _serviceOrderService = serviceOrderService;
        private readonly ILogger<ClientAreaController> _logger = logger;
        
        static ClientAreaController ( )
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet ( "dashboard" )]
        public Task<IActionResult> GetDashboard ( )
        {
            var clientId = GetUserClientId();
            return ExecuteWithLogging (
                "Dashboard",
                clientId,
                ( ) => _clientAreaService.GetClientBasicDashboardAsync ( clientId )
            );
        }


        [HttpGet ( "payments" )]
        public Task<IActionResult> GetPayments ( [FromQuery] PaymentParams parameters )
        {
            var clientId = GetUserClientId();
            parameters.ClientId = clientId;

            return ExecuteWithLogging (
                "Payments",
                clientId,
                ( ) => _paymentService.GetPaginatedForClientAreaAsync ( parameters )
            );
        }


        [HttpGet ( "orders" )]
        public Task<IActionResult> GetClientOrders ( [FromQuery] ServiceOrderParams parameters )
        {
            var clientId = GetUserClientId();
            parameters.ClientId = clientId;

            return ExecuteWithLogging (
                "Orders",
                clientId,
                ( ) => _serviceOrderService.GetPaginatedForClientAreaAsync ( parameters )
            );
        }

        [HttpGet ( "orders/{id}" )]
        public Task<IActionResult> GetOrderDetails ( int id )
        {
            var clientId = GetUserClientId();

            return ExecuteWithLogging (
                $"OrderDetails-{id}",
                clientId,
                ( ) => _serviceOrderService.GetDetailsForClientAreaAsync ( id, clientId )
            );
        }





        [HttpGet ( "invoices" )]
        public Task<IActionResult> GetInvoices ( [FromQuery] InvoiceParams query )
        {
            var clientId = GetUserClientId();
            query.ClientId = clientId;

            return ExecuteWithLogging (
                "Invoices",
                clientId,
                ( ) => _billingService.GetPaginatedInvoicesForClientAreaAsync ( query )
            );
        }




        [HttpGet ( "invoices/{id}/download" )]
        public async Task<IActionResult> DownloadInvoicePdf (
            [FromServices] IBillingService billingService,
            [FromServices] ISystemSettingsService settingsService,
            [FromServices] IMapper mapper,
            [FromServices] IWebHostEnvironment env,
            int id )
        {
            var stopwatch = Stopwatch.StartNew();
            var clientId = GetUserClientId();

            try
            {
                _logger.LogInformation ( "PDF download started for invoice {InvoiceId} by client {ClientId}", id, clientId );

                var invoice = await billingService.GetInvoiceRecordByIdAsync(id);
                if ( invoice == null )
                {
                    _logger.LogWarning ( "Invoice {InvoiceId} not found for client {ClientId}", id, clientId );
                    return NotFound ( "Invoice not found." );
                }

                if ( invoice.ClientId != clientId )
                {
                    _logger.LogWarning ( "Client {ClientId} attempted to access invoice {InvoiceId} belonging to client {OwnerId}", clientId, id, invoice.ClientId );
                    return Forbid ( );
                }

                var settingsEntity = await settingsService.GetAsync();
                var settings = mapper.Map<SystemSettingsRecord>(settingsEntity);

                var logoPath = string.IsNullOrEmpty(settings.LogoUrl)
                    ? null
                    : Path.Combine(env.WebRootPath, "uploads", "logos", Path.GetFileName(settings.LogoUrl));

                var pdf = new BillingInvoicePdfDocument(invoice, settings, logoPath).GeneratePdf();

                stopwatch.Stop ( );
                _logger.LogInformation ( "PDF download completed for invoice {InvoiceId} by client {ClientId} in {ElapsedMs}ms", id, clientId, stopwatch.ElapsedMilliseconds );

                Response.Headers ["Content-Disposition"] = $"inline; filename=invoice-{invoice.InvoiceNumber}.pdf";
                return File ( pdf, "application/pdf" );
            }
            catch ( Exception ex )
            {
                stopwatch.Stop ( );
                _logger.LogError ( ex, "PDF download failed for invoice {InvoiceId} by client {ClientId} after {ElapsedMs}ms", id, clientId, stopwatch.ElapsedMilliseconds );
                return StatusCode ( 500, "An error occurred while generating the PDF." );
            }
        }


        private int GetUserClientId()
        {
            // Buscar o clientId do token JWT
            var clientIdClaim = User.FindFirst("clientId")?.Value;
            
            if (string.IsNullOrEmpty(clientIdClaim))
            {
                _logger.LogError("Token JWT não contém claim 'clientId'");
                throw new UnauthorizedAccessException("Token inválido: clientId não encontrado.");
            }

            if (!int.TryParse(clientIdClaim, out var clientId))
            {
                _logger.LogError("Claim 'clientId' não é um número válido: {ClientIdClaim}", clientIdClaim);
                throw new UnauthorizedAccessException("Token inválido: clientId não é um número válido.");
            }

            if (clientId <= 0)
            {
                _logger.LogError("ClientId inválido: {ClientId}", clientId);
                throw new UnauthorizedAccessException("Token inválido: clientId deve ser maior que zero.");
            }

            _logger.LogDebug("ClientId extraído do token: {ClientId}", clientId);
            return clientId;
        }


        private async Task<IActionResult> ExecuteWithLogging<T> ( string operation, int clientId, Func<Task<T>> action )
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation ( "{Operation} request started for client {ClientId}", operation, clientId );
                var result = await action();
                stopwatch.Stop ( );
                _logger.LogInformation ( "{Operation} request completed for client {ClientId} in {ElapsedMs}ms", operation, clientId, stopwatch.ElapsedMilliseconds );
                return Ok ( result );
            }
            catch ( Exception ex )
            {
                stopwatch.Stop ( );
                _logger.LogError ( ex, "{Operation} request failed for client {ClientId} after {ElapsedMs}ms", operation, clientId, stopwatch.ElapsedMilliseconds );
                throw;
            }
        }
    }
}
