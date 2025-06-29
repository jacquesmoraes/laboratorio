

namespace API.Filters
{
    public class UpdateOverdueStatusFilter ( IScheduleService scheduleService ) : IAsyncActionFilter
    {
        private readonly IScheduleService _scheduleService = scheduleService;

        public async Task OnActionExecutionAsync ( ActionExecutingContext context, ActionExecutionDelegate next )
        {
            await _scheduleService.UpdateOverdueStatusAsync ( );
            await next ( ); 
        }
    }
}
