using Applications.Contracts;
using Applications.PdfDocuments;
using Applications.Projections.Billing;
using Applications.Projections.ServiceOrder;
using Applications.Records.Payments;
using Applications.Records.Settings;
using Applications.Responses;
using AutoMapper;
using Core.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace API.Controllers.Clients
{
    [Authorize ( Roles = "client" )]
    [ApiController]
    [Route ( "api/client-area" )]
    public class ClientAreaController (
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



        static ClientAreaController ( )
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

       

        /// <summary>
        /// Retorna o dashboard resumido do cliente, incluindo dados cadastrais,
        /// totais financeiros e faturas com link para download.
        /// </summary>
        // API/Controllers/Clients/ClientAreaController.cs
        [HttpGet ( "{id}/dashboard" )]
        public async Task<IActionResult> GetDashboard ( int id )
        {
            var dashboard = await _clientAreaService.GetClientBasicDashboardAsync(id);
            return Ok ( dashboard );
        }



        [HttpGet ( "payments" )]
        public async Task<ActionResult<Pagination<ClientPaymentRecord>>> GetPayments (
    [FromQuery] PaymentParams parameters )
        {
            // ⛔ TEMPORÁRIO: aceita clientId da query durante o desenvolvimento
            if ( !parameters.ClientId.HasValue )
                return BadRequest ( "É necessário informar o clientId durante os testes." );

            var result = await _paymentService.GetPaginatedAsync(parameters);
            return Ok ( result );
        }

        [HttpGet ( "orders" )]
        public async Task<ActionResult<Pagination<ServiceOrderListProjection>>> GetClientOrders ( [FromQuery] ServiceOrderParams parameters )
        {
            // Durante os testes, aceitar ClientId pela query
            if ( !parameters.ClientId.HasValue )
                return BadRequest ( "É necessário informar o clientId durante os testes." );

            var result = await _serviceOrderService.GetPaginatedAsync(parameters);
            return Ok ( result );
        }


        [HttpGet ( "invoices" )]
        public async Task<ActionResult<Pagination<BillingInvoiceResponseProjection>>> GetInvoices ( [FromQuery] InvoiceParams query )
        {
            var result = await _billingService.GetPaginatedInvoicesAsync(query);
            return Ok ( result );
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
