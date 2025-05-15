using Core.Models.Shared;

namespace Core.Models.ServiceOrders
{
    public class OrderPayment : PaymentBase
    {

        public List<ServiceOrder> ServiceOrders { get; set; } = [];

    }
}
