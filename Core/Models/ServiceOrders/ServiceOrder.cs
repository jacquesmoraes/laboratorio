using Core.Enums;
using Core.Exceptions;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Works;


namespace Core.Models.ServiceOrders
{
    public class ServiceOrder
    {
        public int ServiceOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public required DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public DateTime? DateOutFinal { get; set; }

        public decimal OrderTotal { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public OrderStatus Status { get; set; }
        public List<ProductionStage> Stages { get; set; } = [];
        public required List<Work> Works { get; set; }
        public int? BillingInvoiceId { get; set; }
        public BillingInvoice? BillingInvoice { get; set; }

        public int ClientId { get; set; }
        public required Client Client { get; set; }
        public bool IsRepeat { get; set; }
        public RepeatResponsible? RepeatResponsible { get; set; }


        public bool IsBillable => Status == OrderStatus.Finished;



        public bool OutForTryInMoreThan ( int days )
        {
            if ( Status != OrderStatus.TryIn )
                return false;

            var lastSend = Stages
        .Where(s => s.DateOut != null)
        .OrderByDescending(s => s.DateOut)
        .FirstOrDefault();

            if ( lastSend?.DateOut == null )
                return false;

            var turnBack = Stages
        .Where(s => s.DateIn > lastSend.DateOut)
        .FirstOrDefault();

            return turnBack == null &&
                   DateTime.UtcNow.Subtract ( lastSend.DateOut.Value ).TotalDays > days;
        }





        public void MoveTo ( Sector sector, DateTime dateIn )
        {
            if ( Status == OrderStatus.Finished )
                throw new BusinessRuleException ( "A ordem já está finalizada." );

            // Finaliza o estágio anterior, se ainda estiver aberto
            var last = Stages.LastOrDefault();
            if ( last != null && last.DateOut == null )
                last.DateOut = dateIn;

            Stages.Add ( new ProductionStage
            {
                Sector = sector,
                SectorId = sector.SectorId,
                DateIn = dateIn,
                ServiceOrder = this
            } );

            Status = OrderStatus.Production;

        }

        public void SendToTryIn ( DateTime dateOut )
        {
            if ( Stages == null || !Stages.Any ( ) )
                throw new BusinessRuleException ( "A ordem de serviço não possui estágios." );

            var openStage = Stages.FirstOrDefault(s => s.DateOut == null);
            if ( openStage == null )
                throw new BusinessRuleException ( "Não há estágio aberto para enviar para prova." );

            openStage.DateOut = dateOut;
            Status = OrderStatus.TryIn;
            DateOut = dateOut;
        }


        public void Finish ( DateTime dateOut )
        {
            var last = Stages.LastOrDefault();
            if ( last == null || last.DateOut != null )
                throw new BusinessRuleException ( "Não há estágio aberto para finalizar." );

            last.DateOut = dateOut;
            Status = OrderStatus.Finished;
            DateOutFinal = dateOut; // ← marca a finalização oficial da OS
        }


    }
}
