using Applications.Dtos.Pricing;
using Core.Models.Pricing;

namespace Applications.Contracts
{
    public interface ITablePriceService : IGenericService<TablePrice>
    {
        Task<TablePrice?> UpdateFromDtoAsync ( UpdateTablePriceDto dto );

    }
}
