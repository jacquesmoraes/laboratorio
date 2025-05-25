using Applications.Dtos.Pricing;
using Core.Models.Pricing;

namespace Applications.Contracts
{
    public interface ITablePriceItemService : IGenericService<TablePriceItem>
    {
        Task<TablePriceItem?> UpdateFromDtoAsync(int id, UpdateTablePriceItemDto dto);

    }
}
