

namespace Applications.Mapping.Resolvers
{
    public class CurrentSectorNameResolver :
        IValueResolver<ServiceOrder, ServiceOrderDetailsProjection, string?>,
        IValueResolver<ServiceOrder, ServiceOrderListProjection, string?>,
        IValueResolver<ServiceOrder, ServiceOrderAlertRecord, string?>
    {
        public string? Resolve ( ServiceOrder src, ServiceOrderDetailsProjection dest, string? member, ResolutionContext context ) => ResolveInternal ( src );
        public string? Resolve ( ServiceOrder src, ServiceOrderListProjection dest, string? member, ResolutionContext context ) => ResolveInternal ( src );
        public string? Resolve ( ServiceOrder src, ServiceOrderAlertRecord dest, string? member, ResolutionContext context ) => ResolveInternal ( src );


        private string? ResolveInternal ( ServiceOrder src )
        {
            var orderedStages = src.Stages
        .OrderByDescending(s => s.DateIn)
        .ToList();

            if ( !orderedStages.Any ( ) )
                return null;

            return src.Status switch
            {
                OrderStatus.Production => orderedStages.FirstOrDefault ( s => s.DateOut == null )?.Sector?.Name,
                OrderStatus.TryIn => orderedStages
                    .Where ( s => s.DateOut != null )
                    .OrderByDescending ( s => s.DateOut )
                    .FirstOrDefault ( )?.Sector?.Name,
                _ => orderedStages
                    .Where ( s => s.DateOut != null )
                    .OrderByDescending ( s => s.DateOut )
                    .FirstOrDefault ( )?.Sector?.Name
            };
        }
    }
}
