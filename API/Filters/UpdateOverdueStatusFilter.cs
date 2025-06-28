using Applications.Contracts;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class UpdateOverdueStatusFilter : IAsyncActionFilter
    {
        private readonly IScheduleService _scheduleService;

        public UpdateOverdueStatusFilter ( IScheduleService scheduleService )
        {
            _scheduleService = scheduleService;
        }

        public async Task OnActionExecutionAsync ( ActionExecutingContext context, ActionExecutionDelegate next )
        {
            await _scheduleService.UpdateOverdueStatusAsync ( ); 
            await next ( ); // Executa a action original
        }
    }
}
