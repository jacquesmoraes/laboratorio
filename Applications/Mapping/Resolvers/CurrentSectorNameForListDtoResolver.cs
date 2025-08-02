namespace Applications.Mapping.Resolvers
{
    public class CurrentSectorNameForListDtoResolver : IValueResolver<ServiceOrder, ServiceOrderListDto, string?>
    {
        public string? Resolve(ServiceOrder src, ServiceOrderListDto dest, string? member, ResolutionContext context)
        {
            var orderedStages = src.Stages
                .OrderByDescending(s => s.DateIn)
                .ToList();

            if (!orderedStages.Any())
                return null;

            return src.Status switch
            {
                OrderStatus.Production => orderedStages.FirstOrDefault(s => s.DateOut == null)?.Sector?.Name,
                OrderStatus.TryIn => orderedStages
                    .Where(s => s.DateOut != null)
                    .OrderByDescending(s => s.DateOut)
                    .FirstOrDefault()?.Sector?.Name,
                _ => orderedStages
                    .Where(s => s.DateOut != null)
                    .OrderByDescending(s => s.DateOut)
                    .FirstOrDefault()?.Sector?.Name
            };
        }
    }
}