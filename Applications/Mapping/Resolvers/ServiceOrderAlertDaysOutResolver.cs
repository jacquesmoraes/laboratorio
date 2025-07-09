namespace Applications.Mapping.Resolvers
{
    public class ServiceOrderAlertDaysOutResolver
        : IValueResolver<ServiceOrder, ServiceOrderAlertRecord, int>
    {
        public int Resolve ( ServiceOrder src, ServiceOrderAlertRecord dest, int member, ResolutionContext context )
        {
            if ( src.Status != OrderStatus.TryIn )
                return 0;

            var tryInStageDate = src.Stages
            .OrderByDescending(s => s.DateIn)
            .FirstOrDefault()?.DateIn;

            if ( tryInStageDate == null )
                return 0;

            return ( DateTime.Now - tryInStageDate.Value ).Days;
        }
    }
}