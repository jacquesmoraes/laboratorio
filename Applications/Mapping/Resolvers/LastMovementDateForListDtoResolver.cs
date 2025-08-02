namespace Applications.Mapping.Resolvers
{
    public class LastMovementDateForListDtoResolver : IValueResolver<ServiceOrder, ServiceOrderListDto, DateTime?>
    {
        public DateTime? Resolve(ServiceOrder src, ServiceOrderListDto dest, DateTime? member, ResolutionContext context)
        {
            if (!src.Stages.Any())
                return null;

           

            var lastStageIn = src.Stages.Max(s => s.DateIn);
            var lastStageOut = src.Stages.Where(s => s.DateOut.HasValue).Max(s => s.DateOut);
            var finishDate = src.DateOutFinal;

            var dates = new List<DateTime> { lastStageIn };

            if (lastStageOut.HasValue)
                dates.Add(lastStageOut.Value);

            if (finishDate.HasValue)
                dates.Add(finishDate.Value);

            return dates.Max();
        }
    }
}