using Core.Enums;
using Core.Models.Clients;
using Core.Models.Works;


namespace Core.Models.ServiceOrders
{
    public class ServiceOrder
    {
        public int ServiceOrderId { get; set; }
        public required int OrderNumber { get; set; }
        public required DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public decimal OrderTotal { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderPayment> Payments { get; set; } = [];
        public List<ProductionStage> Stages { get; set; } = [];
        public required List<Work> Works { get; set; }


        public int ClientId { get; set; }
        public required Client Client { get; set; }

        public decimal AmountPaid => Payments.Sum ( p => p.AmountPaid );

        public decimal RemainingAmount => Math.Max ( 0, OrderTotal - AmountPaid );
        public bool IsBillable => Status == OrderStatus.Finished;

        public bool IsPaid => AmountPaid >= OrderTotal;


    }
}
