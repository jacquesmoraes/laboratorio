namespace API.Controllers.Clients
{
    //[Authorize(Roles = "client")]
    [ApiController]
    [Route("api/client-area")]
    public class ClientAreaController(
        IClientAreaService clientAreaService,
        IPaymentService paymentService,
        IBillingService billingService,
        IServiceOrderService serviceOrderService
    ) : ControllerBase
    {
        private readonly IClientAreaService _clientAreaService = clientAreaService;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IBillingService _billingService = billingService;
        private readonly IServiceOrderService _serviceOrderService = serviceOrderService;

        static ClientAreaController()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Returns the client's summary dashboard, including personal data,
        /// financial totals and invoices with download link.
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userClientId = GetUserClientId();
            var dashboard = await _clientAreaService.GetClientBasicDashboardAsync(userClientId);
            return Ok(dashboard);
        }

        [HttpGet("payments")]
        public async Task<ActionResult<Pagination<ClientPaymentRecord>>> GetPayments([FromQuery] PaymentParams parameters)
        {
            var userClientId = GetUserClientId();
            parameters.ClientId = userClientId;

            var result = await _paymentService.GetPaginatedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("orders")]
        public async Task<ActionResult<Pagination<ServiceOrderListProjection>>> GetClientOrders([FromQuery] ServiceOrderParams parameters)
        {
            var userClientId = GetUserClientId();
            parameters.ClientId = userClientId;

            var result = await _serviceOrderService.GetPaginatedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("invoices")]
        public async Task<ActionResult<Pagination<BillingInvoiceResponseProjection>>> GetInvoices([FromQuery] InvoiceParams query)
        {
            var userClientId = GetUserClientId();
            query.ClientId = userClientId;

            var result = await _billingService.GetPaginatedInvoicesAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Downloads the invoice PDF for the logged-in client.
        /// </summary>
        [HttpGet("invoices/{id}/download")]
        public async Task<IActionResult> DownloadInvoicePdf(
            [FromServices] IBillingService billingService,
            [FromServices] ISystemSettingsService settingsService,
            [FromServices] IMapper mapper,
            [FromServices] IWebHostEnvironment env,
            int id)
        {
            try
            {
                var userClientId = GetUserClientId();

                // Fetch invoice
                var invoice = await billingService.GetInvoiceRecordByIdAsync(id);
                if (invoice == null)
                    return NotFound("Invoice not found.");

                // Validate invoice ownership
                if (invoice.ClientId != userClientId)
                {
                    return Forbid("You can only download your own invoices.");
                }

                // Fetch system settings
                var settingsEntity = await settingsService.GetAsync();
                var settings = mapper.Map<SystemSettingsRecord>(settingsEntity);

                // Resolve absolute logo path
                var logoPath = string.IsNullOrEmpty(settings.LogoUrl)
                    ? null
                    : Path.Combine(env.WebRootPath, "uploads", "logos", Path.GetFileName(settings.LogoUrl));

                // Generate PDF
                var document = new BillingInvoicePdfDocument(invoice, settings, logoPath);
                var pdfBytes = document.GeneratePdf();

                // Return PDF inline
                Response.Headers["Content-Disposition"] = $"inline; filename=invoice-{invoice.InvoiceNumber}.pdf";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to extract clientId from the logged-in user's claims.
        /// </summary>
        /// 
        private int GetUserClientId()
{
    var userClientIdClaim = User.FindFirst("clientId")?.Value;
    if (string.IsNullOrEmpty(userClientIdClaim) || !int.TryParse(userClientIdClaim, out var userClientId))
    {
        // Hardcoded for testing
        userClientId = 6;
    }
    return userClientId;
}

        //private int GetUserClientId()
        //{
        //    var userClientIdClaim = User.FindFirst("clientId")?.Value;
        //    if (string.IsNullOrEmpty(userClientIdClaim) || !int.TryParse(userClientIdClaim, out var userClientId))
        //    {
        //        throw new UnauthorizedAccessException("User does not have a valid clientId.");
        //    }
        //    return userClientId;
        //}
    }
}
