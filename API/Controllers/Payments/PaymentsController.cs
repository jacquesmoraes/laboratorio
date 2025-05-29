using Applications.Contracts;
using Applications.Dtos.Payments;
using AutoMapper;
using Core.FactorySpecifications.PayementSpecifications;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers.Payments
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class PaymentsController (
        IPaymentService clientPaymentService,
        IClientService clientService,
        IUnitOfWork unitOfWork,
        IMapper mapper ) : BaseApiController
    {

        private readonly IPaymentService _clientPaymentService = clientPaymentService;
        private readonly IClientService _clientService = clientService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        // POST: api/payments/client
        /// <summary>
        /// Cria um novo pagamento para um cliente.
        /// </summary>
        /// <param name="dto">Dados do pagamento a ser criado</param>
        /// <returns>Pagamento criado</returns>
        [HttpPost ( "client" )]
        public async Task<IActionResult> CreateClientPayment ( [FromBody] CreatePaymentDto dto )
        {
            try
            {
                var payment = await _clientPaymentService.RegisterClientPaymentAsync(dto);
                var response = _mapper.Map<ClientPaymentDto>(payment);
                return CreatedAtAction ( nameof ( GetClientPaymentById ), new { id = response.Id }, response );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { error = ex.Message } );
            }
        }


        // GET: api/payments/client/{id}
        /// <summary>
        /// Obtém um pagamento de cliente pelo seu ID.
        /// </summary>
        /// <param name="id">ID do pagamento</param>
        /// <returns>Pagamento encontrado</returns>
        [HttpGet ( "client/{id}" )]
        public async Task<IActionResult> GetClientPaymentById ( int id )
        {
            if ( id <= 0 )
                return BadRequest ( "ID do pagamento inválido" );

            var spec = PaymentSpecification.PaymentSpecs.ById(id);
            var payment = await _clientPaymentService.GetEntityWithSpecAsync(spec);

            if ( payment == null )
                return NotFound ( $"Pagamento com ID {id} não encontrado" );

            var dto = _mapper.Map<ClientPaymentDto>(payment);
            return Ok ( dto );
        }


        /// <summary>
        /// Lista os pagamentos de um cliente, com suporte opcional a filtros por mês e ano.
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="month">Mês opcional para filtrar (1-12)</param>
        /// <param name="year">Ano opcional para filtrar</param>
        /// <returns>Lista de pagamentos do cliente</returns>
        [HttpGet ( "client/byclient/{clientId}" )]
        public async Task<IActionResult> GetClientPaymentsWithOptionalPeriod ( int clientId, [FromQuery] int? month, [FromQuery] int? year )
        {
            if ( month.HasValue && ( month < 1 || month > 12 ) )
                return BadRequest ( "O mês deve estar entre 1 e 12" );

            if ( year.HasValue && year < 1 )
                return BadRequest ( "O ano deve ser maior que zero" );

            var (start, end) = GetDateRange ( month, year );
            var spec = start.HasValue && end.HasValue
                ? PaymentSpecification.PaymentSpecs.ByClientAndDateRange(clientId, start.Value, end.Value)
                : PaymentSpecification.PaymentSpecs.ByClientId(clientId);

            var payments = await _clientPaymentService.GetAllWithSpecAsync(spec);
            var dto = _mapper.Map<List<ClientPaymentDto>>(payments);
            return Ok ( dto );
        }

        private static (DateTime? start, DateTime? end) GetDateRange ( int? month, int? year )
        {
            if ( !year.HasValue ) return (null, null);

            if ( month.HasValue )
            {
                var start = new DateTime(year.Value, month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
                return (start, start.AddMonths ( 1 ));
            }

            var yearStart = new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (yearStart, yearStart.AddYears ( 1 ));
        }

    }
}
