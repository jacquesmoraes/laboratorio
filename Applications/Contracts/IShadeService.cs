using Applications.Dtos.Production;
using Core.Models.Production;

namespace Applications.Contracts
{
    public interface IShadeService : IGenericService<Shade>
    {
        Task<Shade> CreateWithValidationAsync(CreateShadeDto dto);
        Task<Shade?> UpdateWithValidationAsync(int id, CreateShadeDto dto);
    }
}
