using Applications.Services;

namespace API.Controllers.ServiceOrders
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceOrdersController(IMapper mapper,
        IServiceOrderService serviceOrderService,
        ITablePriceService tablePriceService
        ) : BaseApiController
    {
        private readonly IServiceOrderService _serviceOrderService = serviceOrderService;
        private readonly ITablePriceService _tablePriceService = tablePriceService;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceOrderDto dto)
        {
            var created = await _serviceOrderService.CreateOrderAsync(dto);
            var response = _mapper.Map<ServiceOrderDetailsProjection>(created);
            return CreatedAtAction(nameof(GetById), new { id = response.ServiceOrderId }, response);
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ServiceOrderListDto>>> GetAll([FromQuery] ServiceOrderParams parameters)
        {
            var response = await _serviceOrderService.GetPaginatedAsync(parameters);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
            var entity = await _serviceOrderService.GetEntityWithSpecAsync(spec);
            if (entity == null) return NotFound();
            var response = _mapper.Map<ServiceOrderDetailsProjection>(entity);
            return Ok(response);
        }

        [HttpPost("moveto")]
        public async Task<IActionResult> MoveToStage([FromBody] MoveToStageDto dto)
        {
            var updated = await _serviceOrderService.MoveToStageAsync(dto);
            if (updated == null) return NotFound();
            var response = _mapper.Map<ServiceOrderDetailsProjection>(updated);
            return Ok(response);
        }

        [HttpPost("tryin")]
        public async Task<IActionResult> SendToTryIn([FromBody] SendToTryInDto dto)
        {
            try
            {
                var updated = await _serviceOrderService.SendToTryInAsync(dto);
                if (updated == null)
                    return NotFound("Service order not found or already finished.");

                var response = _mapper.Map<ServiceOrderDetailsProjection>(updated);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("finish")]
        public async Task<IActionResult> FinishOrders([FromBody] FinishOrderDto dto)
        {
            try
            {
                var result = await _serviceOrderService.FinishOrdersAsync(dto);
                var response = _mapper.Map<List<ServiceOrderDetailsProjection>>(result);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        
        /// <summary>
        /// Returns the price of a specific work type for a given client.
        /// </summary>
        [HttpGet ( "client/{clientId}/worktype/{workTypeId}" )]
        public async Task<IActionResult> GetPriceByClientAndWorkType ( int clientId, int workTypeId )
        {
            var result = await _tablePriceService.GetItemPriceByClientAndWorkTypeAsync(clientId, workTypeId);

            if ( result is null )
                return NotFound ( $"Nenhum preço encontrado para o serviço ID {workTypeId} no cliente ID {clientId}" );

            return Ok ( result );
        }

        [HttpGet("alert/tryin")]
        public async Task<IActionResult> GetWorksOutForTryin([FromQuery] int days = 5)
        {
            var orders = await _serviceOrderService.GetOutForTryInAsync(days);
            var response = _mapper.Map<List<ServiceOrderAlertRecord>>(orders);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateServiceOrderDto dto)
        {
            try
            {
                var updated = await _serviceOrderService.UpdateOrderAsync(id, dto);
                return updated == null
                    ? NotFound()
                    : Ok(_mapper.Map<ServiceOrderDetailsProjection>(updated));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _serviceOrderService.DeleteOrderAsync(id);
                return deleted == null ? NotFound() : NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/reopen")]
        public async Task<IActionResult> Reopen(int id)
        {
            var reopened = await _serviceOrderService.ReopenOrderAsync(id);
            return reopened == null
                ? BadRequest(new { error = "The order cannot be reopened (not found or already in production)." })
                : Ok(_mapper.Map<ServiceOrderDetailsProjection>(reopened));
        }
    }
}
