using Applications.Contracts;
using Applications.Dtos.Payments;
using AutoMapper;
using Core.Interfaces;
using Core.Models.Clients;
using Microsoft.AspNetCore.Mvc;
using static Core.FactorySpecifications.PayementSpecifications.PerClientPaymentSpecification;


namespace API.Controllers.Clients
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClientPaymentsController (
        IGenericService<PerClientPayment> clientPaymentService,
        IClientService clientService,
        IUnitOfWork unitOfWork,
        IMapper mapper ) : BaseApiController
    {

        private readonly IGenericService<PerClientPayment> _clientPaymentService = clientPaymentService;
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
        public async Task<IActionResult> CreateClientPayment ( [FromBody] CreatePerClientPaymentDto dto )
        {
            if ( dto == null )
                return BadRequest ( "Dados do pagamento não fornecidos." );

            if ( dto.AmountPaid <= 0 )
                return BadRequest ( "O valor do pagamento deve ser maior que zero." );

            var client = await _clientService.GetClientIfEligibleForPerClientPayment(dto.ClientId);
            if ( client == null )
                return BadRequest ( "Este cliente não pode realizar pagamentos parciais." );

            using var tx = await _unitOfWork.BeginTransactionAsync();

            // 1. Cria o pagamento
            var entity = _mapper.Map<PerClientPayment>(dto);
            var created = await _clientPaymentService.CreateAsync(entity);

            // 2. Lança o crédito no saldo
            client.Balance.AddCredit ( dto.AmountPaid );
            await _clientService.UpdateAsync ( client.ClientId, client );

            // 3. Salva tudo de uma vez
            await _unitOfWork.SaveChangesAsync ( );
            await tx.CommitAsync ( );

            var response = _mapper.Map<PerClientPaymentDto>(created);
            return CreatedAtAction ( nameof ( GetClientPaymentById ), new { id = response.Id }, response );
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

            var spec = PerClientPaymentSpecs.ById(id);
            var payment = await _clientPaymentService.GetEntityWithSpecAsync(spec);

            if ( payment == null )
                return NotFound ( $"Pagamento com ID {id} não encontrado" );

            var dto = _mapper.Map<PerClientPaymentDto>(payment);
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
                ? PerClientPaymentSpecs.ByClientAndDateRange(clientId, start.Value, end.Value)
                : PerClientPaymentSpecs.ByClientId(clientId);

            var payments = await _clientPaymentService.GetAllWithSpecAsync(spec);
            var dto = _mapper.Map<List<PerClientPaymentDto>>(payments);
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
