namespace API.Controllers.Billing
{
    [Route("api/invoices")]
    [ApiController]
    public class InvoicesController(
        IBillingService billingService,
        ISystemSettingsService settingsService,
        IMapper mapper,
        IWebHostEnvironment env
    ) : BaseApiController
    {
        private readonly IBillingService _billingService = billingService;
        private readonly ISystemSettingsService _settingsService = settingsService;
        private readonly IMapper _mapper = mapper;
        private readonly IWebHostEnvironment _env = env;

        static InvoicesController()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadInvoicePdf(int id)
        {
            try
            {
                // Fetch invoice
                var invoice = await _billingService.GetInvoiceRecordByIdAsync(id);
                if (invoice == null)
                    return NotFound("Invoice not found.");

                // Fetch system settings
                var settingsEntity = await _settingsService.GetAsync();
                var settings = _mapper.Map<SystemSettingsRecord>(settingsEntity);

                // Resolve absolute logo path
                var logoPath = string.IsNullOrEmpty(settings.LogoUrl)
                    ? null
                    : Path.Combine(_env.WebRootPath, "uploads", "logos", Path.GetFileName(settings.LogoUrl));

                // Generate PDF
                var document = new BillingInvoicePdfDocument(invoice, settings, logoPath);
                var pdfBytes = document.GeneratePdf();

                // Return PDF inline
                Response.Headers["Content-Disposition"] = $"inline; filename=invoice-{invoice.InvoiceNumber}.pdf";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log in production via ILogger
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }
    }
}
