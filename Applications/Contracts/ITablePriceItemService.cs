namespace Applications.Contracts
{
    public interface ITablePriceItemService : IGenericService<TablePriceItem>
    {
        Task<TablePriceItem?> UpdateFromDtoAsync ( int id, UpdateTablePriceItemDto dto );

    }
}
