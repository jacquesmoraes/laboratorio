namespace Applications.Mapping.Resolvers
{
    public class ScheduleCurrentSectorNameResolver : IValueResolver<ServiceOrderSchedule, ScheduleItemRecord, string?>
    {
        public string? Resolve ( ServiceOrderSchedule src, ScheduleItemRecord dest, string? member, ResolutionContext context )
        {
            if ( src.ServiceOrder?.Stages == null || !src.ServiceOrder.Stages.Any ( ) )
                return "without sector";

            var orderedStages = src.ServiceOrder.Stages
                .OrderByDescending(s => s.DateIn)
                .ToList();

            return orderedStages.FirstOrDefault ( )?.Sector?.Name ?? "without sector";
        }
    }
}