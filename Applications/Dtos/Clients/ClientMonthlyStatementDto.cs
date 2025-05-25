using Applications.Dtos.Payments;
using Applications.Dtos.ServiceOrder;

namespace Applications.Dtos.Clients
{
    public class ClientMonthlyStatementDto
    {
        public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;

    public List<ServiceOrderShortDto> Orders { get; set; } = new();
    public List<PerClientPaymentDto> Payments { get; set; } = new();

    public decimal TotalOrders => Orders.Sum(x => x.OrderTotal);
    public decimal TotalPaid => Payments.Sum(x => x.AmountPaid);
    public decimal Balance => TotalPaid - TotalOrders;
    }
}
