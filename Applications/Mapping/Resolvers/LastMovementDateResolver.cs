namespace Applications.Mapping.Resolvers
{
    public class LastMovementDateResolver : IValueResolver<ServiceOrder, ServiceOrderListProjection, DateTime?>
    {
        public DateTime? Resolve ( ServiceOrder src, ServiceOrderListProjection dest, DateTime? member, ResolutionContext context )
        {
            if ( !src.Stages.Any ( ) )
                return null;

         
            var lastStageIn = src.Stages.Max(s => s.DateIn);
            var lastStageOut = src.Stages.Where(s => s.DateOut.HasValue).Max(s => s.DateOut);
            var finishDate = src.DateOutFinal;

            var dates = new List<DateTime> { lastStageIn };

            if ( lastStageOut.HasValue )
                dates.Add ( lastStageOut.Value );

            if ( finishDate.HasValue )
                dates.Add ( finishDate.Value );

            return dates.Max ( );
        }
    }
}