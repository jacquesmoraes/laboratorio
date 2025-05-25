using Applications.Dtos.ServiceOrder;
using Core.Models.ServiceOrders;

namespace Applications.Contracts
{
    public interface IServiceOrderService : IGenericService<ServiceOrder>
    {
        Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto );
        Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto );
        Task<IReadOnlyList<ServiceOrder>> GetAllFilteredAsync(ServiceOrderFilterDto filter);
        Task<ServiceOrder?> SendToTryInAsync ( SendToTryInDto dto );
        Task<List<ServiceOrder>> FinishOrdersAsync(FinishOrderDto dto);
        Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto );
        Task<IReadOnlyList<ServiceOrder>> GetOutForTryInAsync ( int days );
        Task<ServiceOrder?> DeleteOrderAsync ( int serviceOrderId );
        Task<ServiceOrder?> ReopenOrderAsync ( int id );

    }
}