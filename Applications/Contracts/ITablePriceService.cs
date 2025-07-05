namespace Applications.Contracts
{
    public interface ITablePriceService : IGenericService<TablePrice>
    {
        Task<TablePrice> CreateFromDtoAsync ( CreateTablePriceDto dto );
        Task<TablePrice?> UpdateFromDtoAsync ( UpdateTablePriceDto dto );

        Task<TablePriceItemRecord?> GetItemPriceByClientAndWorkTypeAsync ( int clientId, int workTypeId );
    }
}
