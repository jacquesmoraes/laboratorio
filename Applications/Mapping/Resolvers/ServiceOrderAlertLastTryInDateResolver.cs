namespace Applications.Mapping.Resolvers
{
    public class ServiceOrderAlertLastTryInDateResolver
        : IValueResolver<ServiceOrder, ServiceOrderAlertRecord, DateTime>
    {
        public DateTime Resolve ( ServiceOrder src, ServiceOrderAlertRecord dest, DateTime member, ResolutionContext context )
        {
            // Se a ordem não está em TryIn, nem faz sentido calcular
            if ( src.Status != OrderStatus.TryIn )
                return DateTime.MinValue;

            // Pega o último Stage registrado (independente do setor) enquanto a OS já estava em TryIn
            var tryInStage = src.Stages
            .OrderByDescending(s => s.DateIn)
            .FirstOrDefault();

            return tryInStage?.DateIn ?? DateTime.MinValue;
        }
    }
}