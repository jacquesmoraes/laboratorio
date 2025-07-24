namespace API.Controllers.Schedule
{
    //[Authorize]
    [ApiController]
    [Route ( "api/schedule" )]
    [ServiceFilter ( typeof ( UpdateOverdueStatusFilter ) )]
    public class ScheduleController ( IScheduleService scheduleService ) : BaseApiController
    {
        private readonly IScheduleService _scheduleService = scheduleService;

        /// <summary>
        /// Schedules a delivery for a service order.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ScheduleItemRecord>> ScheduleDelivery ( ScheduleDeliveryDto dto )
        {
            var result = await _scheduleService.ScheduleDeliveryAsync(dto);
            return Ok ( result );
        }

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        [HttpPut ( "{id}" )]
        public async Task<ActionResult<ScheduleItemRecord>> UpdateSchedule ( int id, ScheduleDeliveryDto dto )
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, dto);
            return result is null ? NotFound ( ) : Ok ( result );
        }


        /// <summary>
        /// Deletes a schedule.
        /// </summary>
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> RemoveSchedule ( int id )
        {
            var success = await _scheduleService.RemoveScheduleAsync(id);
            return success ? NoContent ( ) : NotFound ( );
        }

        /// <summary>
        /// Gets today's schedule.
        /// </summary>
        [HttpGet ( "today" )]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetTodaySchedule ( )
        {
            var result = await _scheduleService.GetTodayScheduleAsync();
            return Ok ( result );
        }

        /// <summary>
        /// Gets the schedule for a specific date.
        /// </summary>
        [HttpGet ( "date/{date:datetime}" )]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetScheduleByDate ( DateTime date )
        {
            var result = await _scheduleService.GetScheduleByDateAsync(date);
            return Ok ( result );
        }

        /// <summary>
        /// Gets the schedule for a date range.
        /// </summary>
        [HttpGet ( "range" )]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetScheduleByDateRange (
            [FromQuery] DateTime start,
            [FromQuery] DateTime end )
        {
            var result = await _scheduleService.GetScheduleByDateRangeAsync(start, end);
            return Ok ( result );
        }

        /// <summary>
        /// Gets the active schedule for a service order.
        /// </summary>
        [HttpGet ( "service-order/{serviceOrderId}" )]
        public async Task<ActionResult<ScheduleItemRecord>> GetActiveScheduleByServiceOrder ( int serviceOrderId )
        {
            var result = await _scheduleService.GetActiveScheduleByServiceOrderIdAsync(serviceOrderId);
            return result is null ? NotFound ( ) : Ok ( result );
        }

        /// <summary>
        /// Gets the scheduled deliveries for a specific sector and date.
        /// </summary>
        [HttpGet ( "current-sector/{sectorId}/date/{date:datetime}" )]
        public async Task<ActionResult<object>> GetCurrentSectorSchedule ( int sectorId, DateTime date )
        {
            var result = await _scheduleService.GetScheduleByCurrentSectorAsync(sectorId, date);

            if ( result is null || !result.Deliveries.Any ( ) )
            {
                return Ok ( new
                {
                    message = "No deliveries scheduled for the selected date.",
                    sectorId,
                    date = date.ToString ( "yyyy-MM-dd" ),
                    deliveries = Array.Empty<ScheduleItemRecord> ( )
                } );
            }

            return Ok ( result );
        }

        /// <summary>
        /// Updates the overdue status of service orders.
        /// </summary>
        [HttpPost ( "update-overdue" )]
        public async Task<IActionResult> UpdateOverdueStatus ( )
        {
            await _scheduleService.UpdateOverdueStatusAsync ( );
            return NoContent ( );
        }
    }
}
