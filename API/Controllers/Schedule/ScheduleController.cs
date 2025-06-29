namespace API.Controllers.Schedule
{
    [Authorize]
    [ServiceFilter(typeof(UpdateOverdueStatusFilter))]
    public class ScheduleController(IScheduleService scheduleService) : BaseApiController
    {
        private readonly IScheduleService _scheduleService = scheduleService;

        /// <summary>
        /// Schedules a delivery for a service order.
        /// </summary>
        [HttpPost("schedule")]
        public async Task<ActionResult<ScheduleItemRecord>> ScheduleDelivery(ScheduleDeliveryDto dto)
        {
            var result = await _scheduleService.ScheduleDeliveryAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        [HttpPut("schedule/{id}")]
        public async Task<ActionResult<ServiceOrderSchedule>> UpdateSchedule(int id, ScheduleDeliveryDto dto)
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, dto);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Deletes a schedule.
        /// </summary>
        [HttpDelete("schedule/{id}")]
        public async Task<IActionResult> RemoveSchedule(int id)
        {
            var success = await _scheduleService.RemoveScheduleAsync(id);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Gets today's schedule.
        /// </summary>
        [HttpGet("today")]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetTodaySchedule()
        {
            var result = await _scheduleService.GetTodayScheduleAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets the schedule for a specific date.
        /// </summary>
        [HttpGet("date/{date:datetime}")]
        public async Task<ActionResult<List<SectorScheduleRecord>>> GetScheduleByDate(DateTime date)
        {
            var result = await _scheduleService.GetScheduleByDateAsync(date);
            return Ok(result);
        }

        /// <summary>
        /// Gets the scheduled deliveries for a specific sector and date.
        /// </summary>
        [HttpGet("current-sector/{sectorId}/date/{date:datetime}")]
        public async Task<ActionResult<object>> GetCurrentSectorSchedule(int sectorId, DateTime date)
        {
            var result = await _scheduleService.GetScheduleByCurrentSectorAsync(sectorId, date);

            if (result is null || !result.Deliveries.Any())
            {
                return Ok(new
                {
                    message = "No deliveries scheduled for the selected date.",
                    sectorId,
                    date = date.ToString("yyyy-MM-dd"),
                    deliveries = Array.Empty<ScheduleItemRecord>()
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates the overdue status of service orders.
        /// </summary>
        [HttpPost("update-overdue")]
        public async Task<IActionResult> UpdateOverdueStatus()
        {
            await _scheduleService.UpdateOverdueStatusAsync();
            return NoContent();
        }
    }
}
