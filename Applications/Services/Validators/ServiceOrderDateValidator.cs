using Core.Exceptions;
using Core.Models.ServiceOrders;

namespace Applications.Services.Validators
{
    public static class ServiceOrderDateValidator
    {
        public static void ValidateNewStageDate ( List<ProductionStage> stages, DateTime newDateIn )
        {
            var lastStage = stages.OrderByDescending(s => s.DateIn).FirstOrDefault();
            if ( lastStage != null && newDateIn < lastStage.DateOut.GetValueOrDefault ( lastStage.DateIn ) )
            {
                throw new UnprocessableEntityException ( "A nova movimentação deve ocorrer após a última movimentação registrada." );
            }
        }

        public static void ValidateTryInDate ( List<ProductionStage> stages, DateTime tryInDate )
        {
            var lastStage =  stages.OrderByDescending(s => s.DateIn).FirstOrDefault()  
                ?? throw new UnprocessableEntityException ( "Não é possível enviar para prova sem movimentações anteriores." );
            if ( tryInDate < lastStage.DateOut.GetValueOrDefault ( lastStage.DateIn ) )
                throw new UnprocessableEntityException ( "A data de envio para prova deve ser posterior à última movimentação." );
        }

        public static void ValidateFinishDate ( ServiceOrder order, DateTime finishDate )
        {
            if ( finishDate < order.DateIn )
                throw new UnprocessableEntityException ( "A data de finalização não pode ser anterior à data de entrada." );

            var lastStageOut = order.Stages
            .Where(s => s.DateOut.HasValue)
            .OrderByDescending(s => s.DateOut)
            .FirstOrDefault()?.DateOut;

            if ( lastStageOut.HasValue && finishDate < lastStageOut.Value )
                throw new UnprocessableEntityException ( "A data de finalização deve ser posterior à última saída registrada." );
        }
    }
}