using Applications.Dtos.ServiceOrder;
using Applications.Projections.ServiceOrder;
using Applications.Responses;
using Core.Models.ServiceOrders;
using Core.Params;

namespace Applications.Contracts
{
    public interface IServiceOrderService : IGenericService<ServiceOrder>
    {
        Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto );
        Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto );
       
        Task<ServiceOrder?> SendToTryInAsync ( SendToTryInDto dto );
        Task<List<ServiceOrder>> FinishOrdersAsync(FinishOrderDto dto);
        Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto );
        Task<Pagination<ServiceOrderListProjection>> GetPaginatedAsync(ServiceOrderParams p);

        Task<IReadOnlyList<ServiceOrder>> GetOutForTryInAsync ( int days );
        Task<ServiceOrder?> DeleteOrderAsync ( int serviceOrderId );
        Task<ServiceOrder?> ReopenOrderAsync ( int id );

    }
}