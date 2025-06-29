namespace Applications.Services
{
    public class TablePriceItemService ( IGenericRepository<TablePriceItem> repository ) 
        : GenericService<TablePriceItem> ( repository ), ITablePriceItemService
    {
        private readonly IGenericRepository<TablePriceItem> _repository = repository;

        public async Task<TablePriceItem?> UpdateFromDtoAsync ( int id, UpdateTablePriceItemDto dto )
        {
            if ( id <= 0 )
                throw new CustomValidationException ( "Invalid table price item ID." );

            if ( string.IsNullOrWhiteSpace ( dto.ItemName ) )
                throw new CustomValidationException ( "Item name is required." );

            if ( dto.Price < 0 )
                throw new CustomValidationException ( "Price cannot be negative." );

            var existing = await _repository.GetEntityWithSpec(TablePriceItemSpecs.ByIdWithRelations(id))
            ?? throw new NotFoundException($"Table price item with ID {id} not found.");

            existing.ItemName = dto.ItemName;
            existing.Price = dto.Price;

            var updated = await _repository.UpdateAsync(id, existing)
            ?? throw new BusinessRuleException("Failed to update table price item.");

            return updated;
        }
    }
}
