namespace Applications.Services.Validators
{
    public static class ServiceOrderDateValidator
    {
        public static void ValidateNewStageDate(List<ProductionStage> stages, DateTime newDateIn)
        {
            var lastStage = stages.OrderByDescending(s => s.DateIn).FirstOrDefault();
            if (lastStage != null && newDateIn < lastStage.DateOut.GetValueOrDefault(lastStage.DateIn))
            {
                throw new UnprocessableEntityException("The new stage must occur after the last recorded stage.");
            }
        }

        public static void ValidateTryInDate(List<ProductionStage> stages, DateTime tryInDate)
        {
            var lastStage = stages.OrderByDescending(s => s.DateIn).FirstOrDefault()
                ?? throw new UnprocessableEntityException("Cannot send to try-in without prior stages.");

            if (tryInDate < lastStage.DateOut.GetValueOrDefault(lastStage.DateIn))
                throw new UnprocessableEntityException("Try-in date must be after the last recorded stage.");
        }

        public static void ValidateFinishDate(ServiceOrder order, DateTime finishDate)
        {
            if (finishDate < order.DateIn)
                throw new UnprocessableEntityException("Finish date cannot be earlier than the entry date.");

            var lastStageOut = order.Stages
                .Where(s => s.DateOut.HasValue)
                .OrderByDescending(s => s.DateOut)
                .FirstOrDefault()?.DateOut;

            if (lastStageOut.HasValue && finishDate < lastStageOut.Value)
                throw new UnprocessableEntityException("Finish date must be after the last stage out date.");
        }
    }
}
