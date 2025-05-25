using Applications.Contracts;
using Applications.Dtos.Billing;
using AutoMapper;
using Core.FactorySpecifications.Billing;
using Core.Models.Billing;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateBillingInvoiceDto dto )
        {
            try
            {
                var invoice = await _billingService.GenerateInvoiceAsync(dto);
                var response = _mapper.Map<BillingInvoiceResponseDto>(invoice);
                return CreatedAtAction ( nameof ( GetById ), new { id = response.BillingInvoiceId }, response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( ex.Message );
            }
        }


        [HttpPost ( "{id}/pay" )]
        public async Task<IActionResult> MarkAsPaid ( int id )
        {
            try
            {
                var invoice = await _billingService.MarkAsPaidAsync(id);
                var response = _mapper.Map<BillingInvoiceResponseDto>(invoice);
                return Ok ( response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( ex.Message );
            }
        }

        [HttpPost ( "{id}/cancel" )]
        public async Task<IActionResult> Cancel ( int id )
        {
            try
            {
                var invoice = await _billingService.CancelInvoiceAsync(id);
                var response = _mapper.Map<BillingInvoiceResponseDto>(invoice);
                return Ok ( response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( ex.Message );
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByClient ( [FromQuery] int clientId )
        {
            var invoices = await _billingService.GetAllByClientAsync(clientId);
            var response = _mapper.Map<List<BillingInvoiceResponseDto>>(invoices);
            return Ok ( response );
        }


        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = BillingInvoiceSpecification.BillingInvoiceSpecs.ByIdFull(id);
            var invoice = await _billingService.GetEntityWithSpecAsync(spec);
            if ( invoice == null ) return NotFound ( );

            var response = _mapper.Map<BillingInvoiceResponseDto>(invoice);
            return Ok ( response );
        }


    }
}
