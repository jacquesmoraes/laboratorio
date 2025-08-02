using Applications.Projections.ClientArea;

namespace Applications.Contracts
{
    public interface IServiceOrderService : IGenericService<ServiceOrder>
    {
        Task<ServiceOrder> CreateOrderAsync ( CreateServiceOrderDto dto );
        Task<ServiceOrder?> MoveToStageAsync ( MoveToStageDto dto );
        Task<Pagination<ServiceOrderListDto>> GetPaginatedLightAsync (ServiceOrderParams p  );
        Task<ServiceOrder?> SendToTryInAsync ( SendToTryInDto dto );
        Task<List<ServiceOrder>> FinishOrdersAsync ( FinishOrderDto dto );
        Task<ServiceOrder?> UpdateOrderAsync ( int id, CreateServiceOrderDto dto );
        Task<Pagination<ServiceOrderListProjection>> GetPaginatedAsync ( ServiceOrderParams p );
        Task<Pagination<ClientAreaServiceOrderProjection>> GetPaginatedForClientAreaAsync(ServiceOrderParams p);
        Task<IReadOnlyList<ServiceOrder>> GetOutForTryInAsync ( int days );
        Task<ClientAreaServiceOrderDetailsProjection?> GetDetailsForClientAreaAsync(int serviceOrderId, int clientId);
        Task<ServiceOrder?> DeleteOrderAsync ( int serviceOrderId );
        Task<ServiceOrder?> ReopenOrderAsync ( int id );

    }
}