using Applications.Services;

namespace API.Controllers.ServiceOrders
{
    [ApiController]
    [Route ( "api/[controller]" )]
    public class ServiceOrdersController ( IMapper mapper,
        IServiceOrderService serviceOrderService,
        ITablePriceService tablePriceService
,
        IPerformanceLoggingService performanceLoggingService ) : BaseApiController
    {
        private readonly IServiceOrderService _serviceOrderService = serviceOrderService;
        private readonly ITablePriceService _tablePriceService = tablePriceService;
        private readonly IMapper _mapper = mapper;
        private readonly IPerformanceLoggingService _performanceLoggingService = performanceLoggingService;

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateServiceOrderDto dto )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_CreateServiceOrder", new Dictionary<string, object>
            {
                ["ClientId"] = dto.ClientId,
                ["PatientName"] = dto.PatientName,
                ["WorksCount"] = dto.Works.Count
            } ) )
            {
                var created = await _serviceOrderService.CreateOrderAsync(dto);
                var response = _mapper.Map<ServiceOrderDetailsProjection>(created);
                return CreatedAtAction ( nameof ( GetById ), new { id = response.ServiceOrderId }, response );
            }
        }


        [HttpGet]
        public async Task<ActionResult<Pagination<ServiceOrderListDto>>> GetAll ( [FromQuery] ServiceOrderParams parameters )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_GetAllServiceOrders", new Dictionary<string, object>
            {
                ["PageNumber"] = parameters.PageNumber,
                ["PageSize"] = parameters.PageSize,
                ["HasFilters"] = !string.IsNullOrEmpty ( parameters.PatientName ) ||
                                 parameters.ClientId.HasValue ||
                                 parameters.Status.HasValue
            } ) )
            {
                var response = await _serviceOrderService.GetPaginatedLightAsync(parameters);
                return Ok ( response );
            }
        }


        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_GetServiceOrderById", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = id
            } ) )
            {
                var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
                var entity = await _serviceOrderService.GetEntityWithSpecAsync(spec);
                if ( entity == null ) return NotFound ( );
                var response = _mapper.Map<ServiceOrderDetailsProjection>(entity);
                return Ok ( response );
            }
        }

        [HttpPost ( "moveto" )]
        public async Task<IActionResult> MoveToStage ( [FromBody] MoveToStageDto dto )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_MoveToStage", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = dto.ServiceOrderId,
                ["SectorId"] = dto.SectorId
            } ) )
            {
                var updated = await _serviceOrderService.MoveToStageAsync(dto);
                if ( updated == null ) return NotFound ( );
                var response = _mapper.Map<ServiceOrderDetailsProjection>(updated);
                return Ok ( response );
            }
        }

        [HttpPost ( "tryin" )]
        public async Task<IActionResult> SendToTryIn ( [FromBody] SendToTryInDto dto )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_SendToTryIn", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = dto.ServiceOrderId
            } ) )
            {
                var result = await _serviceOrderService.SendToTryInAsync(dto);
                // ✅ CORRIGIDO: Usar mapper para evitar referência circular
                var response = _mapper.Map<ServiceOrderDetailsProjection>(result);
                return Ok ( response );
            }
        }

        [HttpPost ( "finish" )]
        public async Task<IActionResult> FinishOrders ( [FromBody] FinishOrderDto dto )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_FinishOrders", new Dictionary<string, object>
            {
                ["ServiceOrderIdsCount"] = dto.ServiceOrderIds.Count
            } ) )
            {
                var result = await _serviceOrderService.FinishOrdersAsync(dto);
                var response = _mapper.Map<List<ServiceOrderDetailsProjection>>(result);
                return Ok ( response );
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
                return NotFound ( $"No price found for service ID {workTypeId} in client ID {clientId}" );

            return Ok ( result );
        }

        [HttpGet ( "alert/tryin" )]
        public async Task<IActionResult> GetWorksOutForTryin ( [FromQuery] int days = 30 )
        {
            var orders = await _serviceOrderService.GetOutForTryInAsync(days);
            var response = _mapper.Map<List<ServiceOrderAlertRecord>>(orders);
            return Ok ( response );
        }



        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] CreateServiceOrderDto dto )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_UpdateServiceOrder", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = id,
                ["WorksCount"] = dto.Works?.Count ?? 0
            } ) )
            {
                var updated = await _serviceOrderService.UpdateOrderAsync(id, dto);
                return Ok ( _mapper.Map<ServiceOrderDetailsProjection> ( updated ) );
            }
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_DeleteServiceOrder", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = id
            } ) )
            {
                var deleted = await _serviceOrderService.DeleteOrderAsync(id);
                return deleted == null ? NotFound ( ) : NoContent ( );
            }
        }

        [HttpPost ( "{id}/reopen" )]
        public async Task<IActionResult> Reopen ( int id )
        {
            using ( _performanceLoggingService.MeasureOperation ( "Controller_ReopenServiceOrder", new Dictionary<string, object>
            {
                ["ServiceOrderId"] = id
            } ) )
            {
                var reopened = await _serviceOrderService.ReopenOrderAsync(id);
                return Ok ( _mapper.Map<ServiceOrderDetailsProjection> ( reopened ) );
            }
        }
    }
}
