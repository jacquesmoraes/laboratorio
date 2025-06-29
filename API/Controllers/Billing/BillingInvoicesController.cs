namespace API.Controllers.Billing
{
    [ApiController]
    [Route ( "api/[controller]" )]
    public class BillingInvoicesController (
        IBillingService billingService,
        IMapper mapper
    ) : BaseApiController
    {
        private readonly IBillingService _billingService = billingService;
        private readonly IMapper _mapper = mapper;

        [HttpPost ( "invoice" )]
        public async Task<IActionResult> CreateInvoice ( [FromBody] CreateBillingInvoiceDto dto )
        {
            if ( dto.ServiceOrderIds == null || dto.ServiceOrderIds.Count == 0 )
                return BadRequest ( "At least one service order is required." );

            var invoice = await _billingService.GenerateInvoiceAsync(dto);
            var response = _mapper.Map<BillingInvoiceResponseProjection>(invoice);
            return Ok ( response );
        }

        [HttpPost ( "{id}/cancel" )]
        public async Task<IActionResult> Cancel ( int id )
        {
            try
            {
                var invoice = await _billingService.CancelInvoiceAsync(id);
                var response = _mapper.Map<BillingInvoiceResponseProjection>(invoice);
                return Ok ( response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( ex.Message );
            }
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.ByIdFull(id);
            var invoice = await _billingService.GetEntityWithSpecAsync(spec);
            if ( invoice == null ) return NotFound ( );

            var response = _mapper.Map<BillingInvoiceResponseProjection>(invoice);
            return Ok ( response );
        }
    }
}
