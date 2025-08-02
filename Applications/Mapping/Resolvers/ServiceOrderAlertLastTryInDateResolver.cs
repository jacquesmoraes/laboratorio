namespace Applications.Mapping.Resolvers
{
    public class ServiceOrderAlertLastTryInDateResolver
        : IValueResolver<ServiceOrder, ServiceOrderAlertRecord, DateTime>
    {
        public DateTime Resolve ( ServiceOrder src, ServiceOrderAlertRecord dest, DateTime member, ResolutionContext context )
        {
            //if the order is not in TryIn, it doesn't make sense to calculate
            
            if ( src.Status != OrderStatus.TryIn )
                return DateTime.MinValue;

            //take the least stage registered independent of the sector while the service order was in TryIn
            
            var tryInStage = src.Stages
            .OrderByDescending(s => s.DateIn)
            .FirstOrDefault();

            return tryInStage?.DateIn ?? DateTime.MinValue;
        }
    }
}