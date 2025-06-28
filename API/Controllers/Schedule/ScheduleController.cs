using API.Filters;
using Applications.Contracts;
using Applications.Dtos.Schedule;
using Applications.Records.Schedule;
using Core.Models.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Schedule
{
    [Authorize]
    [ServiceFilter ( typeof ( UpdateOverdueStatusFilter ) )]
    public class ScheduleController : BaseApiController
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController ( IScheduleService scheduleService )
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Agenda uma entrega para uma OS
        /// </summary>
        [HttpPost ( "schedule" )]
        public async Task<ActionResult<ScheduleItemRecord>> ScheduleDelivery ( ScheduleDeliveryDto dto )
        {
            try
            {
                var result = await _scheduleService.ScheduleDeliveryAsync(dto);
                return Ok ( result );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }

        /// <summary>
        /// Atualiza um agendamento existente
        /// </summary>
        [HttpPut ( "schedule/{id}" )]
        public async Task<ActionResult<ServiceOrderSchedule>> UpdateSchedule ( int id, ScheduleDeliveryDto dto )
        {
            try
            {
                var result = await _scheduleService.UpdateScheduleAsync(id, dto);
                if ( result == null ) return NotFound ( );
                return Ok ( result );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }

        /// <summary>
        /// Remove um agendamento
        /// </summary>
        [HttpDelete ( "schedule/{id}" )]
        public async Task<ActionResult> RemoveSchedule ( int id )
        {
            try
            {
                var result = await _scheduleService.RemoveScheduleAsync(id);
                if ( !result ) return NotFound ( );
                return NoContent ( );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }

        /// <summary>
        /// Obtém a agenda de hoje
        /// </summary>
        [HttpGet ( "today" )]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetTodaySchedule ( )
        {
            try
            {
                var result = await _scheduleService.GetTodayScheduleAsync();
                return Ok ( result );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }

        /// <summary>
        /// Obtém a agenda para uma data específica
        /// </summary>
        [HttpGet ( "date/{date:datetime}" )]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetScheduleByDate ( DateTime date )
        {
            try
            {
                var result = await _scheduleService.GetScheduleByDateAsync(date);
                return Ok ( result );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }

        [HttpGet ( "current-sector/{sectorId}/date/{date:datetime}" )]
        public async Task<ActionResult<SectorScheduleRecord>> GetCurrentSectorSchedule ( int sectorId, DateTime date )
        {
            try
            {
                var result = await _scheduleService.GetScheduleByCurrentSectorAsync(sectorId, date);

                if ( result is null || !result.Deliveries.Any ( ) )
                {
                    return Ok ( new
                    {
                        message = "Sem entregas agendadas para a data informada.",
                        sectorId = sectorId,
                        date = date.ToString ( "yyyy-MM-dd" ),
                        deliveries =  Array.Empty<ScheduleItemRecord>()
                    } );
                }

                return Ok ( result );

            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }





        /// <summary>
        /// Atualiza o status de atraso das OS
        /// </summary>
        [HttpPost ( "update-overdue" )]
        public async Task<ActionResult> UpdateOverdueStatus ( )
        {
            try
            {
                await _scheduleService.UpdateOverdueStatusAsync ( );
                return NoContent ( );
            }
            catch ( Exception ex )
            {
                return BadRequest ( new { message = ex.Message } );
            }
        }


    }
}