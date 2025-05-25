using Applications.Dtos.Sector;
using Core.Models.ServiceOrders;

namespace Applications.Contracts
{
    public interface ISectorService : IGenericService<Sector>
    {
        Task<Sector?> UpdateFromDtoAsync(UpdateSectorDto dto);
    }
}
