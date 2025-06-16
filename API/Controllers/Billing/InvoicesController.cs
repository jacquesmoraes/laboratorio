using Applications.Contracts;
using Applications.PdfDocuments;
using Applications.Records.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace API.Controllers.Billing
{
    [Route ( "api/invoices" )]
    [ApiController]
    public class InvoicesController (
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

        static InvoicesController ( )
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet ( "{id}/pdf" )]
        public async Task<IActionResult> DownloadInvoicePdf ( int id )
        {
            try
            {
                // Buscar fatura
                var invoice = await _billingService.GetInvoiceRecordByIdAsync(id);
                if ( invoice == null )
                    return NotFound ( "Fatura não encontrada." );

                // Buscar configurações
                var settingsEntity = await _settingsService.GetAsync();
                var settings = _mapper.Map<SystemSettingsRecord>(settingsEntity);

                // Resolver caminho absoluto do logo
                var logoPath = string.IsNullOrEmpty(settings.LogoUrl)
                    ? null
                    : Path.Combine(_env.WebRootPath, "uploads", "logos", Path.GetFileName(settings.LogoUrl));

                // Gerar PDF
                var document = new BillingInvoicePdfDocument(invoice, settings, logoPath);
                var pdfBytes = document.GeneratePdf();

                // Retornar PDF inline
                Response.Headers ["Content-Disposition"] = $"inline; filename=fatura-{invoice.InvoiceNumber}.pdf";
                return File ( pdfBytes, "application/pdf" );

            }
            catch ( Exception ex )
            {
                // Log em produção via ILogger
                return StatusCode ( 500, $"Erro ao gerar PDF: {ex.Message}" );
            }
        }
    }
}
