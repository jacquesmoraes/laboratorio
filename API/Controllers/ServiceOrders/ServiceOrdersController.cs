using Applications.Contracts;
using Applications.Dtos.ServiceOrder;
using AutoMapper;
using Core.FactorySpecifications.ServiceOrderFactorySpecifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.ServiceOrders
{
    [ApiController]
    [Route ( "api/[controller]" )]
    public class ServiceOrdersController ( IMapper mapper, IServiceOrderService service ) : BaseApiController
    {
        private readonly IServiceOrderService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateServiceOrderDto dto )
        {
            var created = await _service.CreateOrderAsync(dto);
            var response = _mapper.Map<ServiceOrderResponseDto>(created);

            return CreatedAtAction ( nameof ( GetById ), new { id = response.ServiceOrderId }, response );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ( [FromQuery] ServiceOrderFilterDto filter )
        {

            var result = await _service.GetAllFilteredAsync(filter);
            var response = _mapper.Map<List<ServiceOrderResponseDto>>(result);
            return Ok ( response );
        }



        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {

            var spec = ServiceOrderSpecification.ServiceOrderSpecs.ByIdFull(id);
            var entity = await _service.GetEntityWithSpecAsync(spec);

            if ( entity == null ) return NotFound ( );

            var response = _mapper.Map<ServiceOrderDetailsDto>(entity);
            return Ok ( response );
        }

        [HttpPost ( "moveto" )]
        public async Task<IActionResult> MoveToStage ( [FromBody] MoveToStageDto dto )
        {
            var updated = await _service.MoveToStageAsync(dto);
            if ( updated == null )
                return NotFound ( );

            var response = _mapper.Map<ServiceOrderResponseDto>(updated);
            return Ok ( response );
        }



        [HttpPost ( "tryin" )]
        public async Task<IActionResult> SendToTryIn ( [FromBody] SendToTryInDto dto )
        {
            var updated = await _service.SendToTryInAsync(dto);

            if ( updated == null )
                return NotFound ( "Ordem de serviço não encontrada ou já finalizada." );

            var response = _mapper.Map<ServiceOrderResponseDto>(updated);
            return Ok ( response );
        }


        [HttpPost ( "finish" )]
        public async Task<IActionResult> FinishOrders ( [FromBody] FinishOrderDto dto )
        {
            try
            {
                var result = await _service.FinishOrdersAsync(dto);
                return Ok ( _mapper.Map<List<ServiceOrderResponseDto>> ( result ) );
            }
            catch ( InvalidOperationException ex )
            {
                return BadRequest ( new { error = ex.Message } );
            }
        }


        [HttpGet ( "alert/tryin" )]
        public async Task<IActionResult> GetWorksOutForTryin ( [FromQuery] int dias = 5 )
        {
            var ordens = await _service.GetOutForTryInAsync(dias);
            var response = _mapper.Map<List<ServiceOrderAlertDto>>(ordens);
            return Ok ( response );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] CreateServiceOrderDto dto )
        {
            try
            {
                var updated = await _service.UpdateOrderAsync(id, dto);
                return updated == null
                    ? NotFound ( )
                    : Ok ( _mapper.Map<ServiceOrderResponseDto> ( updated ) );
            }
            catch ( InvalidOperationException ex )
            {
                return BadRequest ( new { error = ex.Message } );
            }
        }


        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            try
            {
                var deleted = await _service.DeleteOrderAsync(id);
                return deleted == null ? NotFound ( ) : NoContent ( );
            }
            catch ( InvalidOperationException ex )
            {
                return BadRequest ( new { error = ex.Message } );
            }
        }

        [HttpPost ( "{id}/reopen" )]
        public async Task<IActionResult> Reopen ( int id )
        {
            var reopened = await _service.ReopenOrderAsync(id);
            return reopened == null
                ? BadRequest ( new { error = "A ordem não pode ser reaberta (não encontrada ou já está em produção)." } )
                : Ok ( _mapper.Map<ServiceOrderResponseDto> ( reopened ) );
        }


    }

}
