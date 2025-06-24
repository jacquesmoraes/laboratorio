using Applications.Contracts;
using Applications.Dtos.Payments;
using Applications.Records.Payments;
using Applications.Responses;
using Applications.Services;
using AutoMapper;
using Core.FactorySpecifications.PaymentSpecifications;
using Core.Params;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Payments
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class PaymentsController (
        IPaymentService clientPaymentService,
        IMapper mapper ) : BaseApiController
    {
        private readonly IPaymentService _clientPaymentService = clientPaymentService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Cria um novo pagamento para um cliente.
        /// </summary>
        [HttpPost ( "client" )]
        public async Task<IActionResult> CreateClientPayment ( [FromBody] CreatePaymentDto dto )
        {
            try
            {
                var payment = await _clientPaymentService.RegisterClientPaymentAsync(dto);
                var response = _mapper.Map<ClientPaymentRecord>(payment);
                return CreatedAtAction ( nameof ( GetClientPaymentById ), new { id = response.Id }, response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { error = ex.Message } );
            }
        }
        /// <summary>
        /// Retorna a lista paginada de pagamentos com filtros e ordenação.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Pagination<ClientPaymentRecord>>> GetAll([FromQuery] PaymentParams parameters)
        {
            var result = await _clientPaymentService.GetPaginatedAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Obtém um pagamento de cliente pelo seu ID.
        /// </summary>
        [HttpGet ( "client/{id}" )]
        public async Task<IActionResult> GetClientPaymentById ( int id )
        {
            if ( id <= 0 )
                return BadRequest ( "ID do pagamento inválido" );

            var spec = PaymentSpecification.PaymentSpecs.ById(id);
            var payment = await _clientPaymentService.GetEntityWithSpecAsync(spec);

            if ( payment == null )
                return NotFound ( $"Pagamento com ID {id} não encontrado" );

            var dto = _mapper.Map<ClientPaymentRecord>(payment);
            return Ok ( dto );
        }

        
    }
}
