using Applications.Contracts;
using Applications.PdfDocuments;
using Applications.Records.Clients;
using Applications.Records.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace API.Controllers.Clients
{
    [ApiController]
    [Route ( "api/client-area" )]
    public class ClientAreaController (
        IClientAreaService clientAreaService,
        IMapper mapper ) : ControllerBase
    {
        private readonly IClientAreaService _clientAreaService = clientAreaService;
        private readonly IMapper _mapper = mapper;


         static ClientAreaController ( )
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Retorna o dashboard completo do cliente, incluindo dados cadastrais,
        /// totais financeiros e faturas com link para download.
        /// </summary>
        [HttpGet ( "{id}/dashboard" )]
        public async Task<IActionResult> GetDashboard ( int id,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate )
        {
            var data = await _clientAreaService.GetClientDashboardAsync(id, startDate, endDate);

            var response = _mapper.Map<ClientAreaRecord>(data);
            return Ok ( response );
        }

        /// <summary>
        /// Faz o download do PDF da fatura para o cliente.
        /// </summary>
        [HttpGet ( "invoices/{id}/download" )]
        public async Task<IActionResult> DownloadInvoicePdf (
            [FromServices] IBillingService billingService,
            [FromServices] ISystemSettingsService settingsService,
            [FromServices] IMapper mapper,
            [FromServices] IWebHostEnvironment env,
            int id )
        {
            try
            {
                // Buscar fatura
                var invoice = await billingService.GetInvoiceRecordByIdAsync(id);
                if ( invoice == null )
                    return NotFound ( "Fatura não encontrada." );

                // Buscar configurações do sistema
                var settingsEntity = await settingsService.GetAsync();
                var settings = mapper.Map<SystemSettingsRecord>(settingsEntity);

                // Caminho absoluto do logo
                var logoPath = string.IsNullOrEmpty(settings.LogoUrl)
            ? null
            : Path.Combine(env.WebRootPath, "uploads", "logos", Path.GetFileName(settings.LogoUrl));

                // Criar PDF
                var document = new BillingInvoicePdfDocument(invoice, settings, logoPath);
                var pdfBytes = document.GeneratePdf();

                // Retornar PDF inline
                Response.Headers ["Content-Disposition"] = $"inline; filename=fatura-{invoice.InvoiceNumber}.pdf";
                return File ( pdfBytes, "application/pdf" );

            }
            catch ( Exception ex )
            {
                return StatusCode ( 500, $"Erro ao gerar PDF: {ex.Message}" );
            }
        }


    }
}
