namespace Applications.Services
{
    public class TablePriceService (
        IGenericRepository<TablePrice> repository,
        IGenericRepository<TablePriceItem> itemRepository
    ) : GenericService<TablePrice> ( repository ), ITablePriceService
    {
        private readonly IGenericRepository<TablePrice> _repository = repository;
        private readonly IGenericRepository<TablePriceItem> _itemRepository = itemRepository;

        public async Task<TablePrice> CreateFromDtoAsync ( CreateTablePriceDto dto )
        {
            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "Table price name is required." );

            if ( dto.Items == null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "The table price must contain at least one item." );

            var duplicates = dto.Items
                .GroupBy(i => i.TablePriceItemId)
                .Where(g => g.Count() > 1)
                .ToList();

            if ( duplicates.Any ( ) )
                throw new BusinessRuleException ( $"Duplicate items detected with ID(s): {string.Join ( ", ", duplicates.Select ( d => d.Key ) )}" );

            var itemIds = dto.Items.Select(i => i.TablePriceItemId).ToList();

            // Use specification to fetch items by their IDs
            var spec = TablePriceItemSpecs.ByIds(itemIds);
            var existingItems = await _itemRepository.GetAllAsync(spec);

            if ( existingItems.Count != itemIds.Count )
                throw new NotFoundException ( "One or more TablePriceItems were not found." );

            var tablePrice = new TablePrice
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = false,
                Items = existingItems.ToList ( )
            };

            await _repository.CreateAsync ( tablePrice );

            return tablePrice;
        }

        public async Task<TablePrice?> UpdateFromDtoAsync ( UpdateTablePriceDto dto )
        {
            if ( dto.Id <= 0 )
                throw new CustomValidationException ( "Invalid table price ID." );

            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "Table price name is required." );

            if ( dto.Items == null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "The table price must contain at least one item." );

            var spec = TablePriceSpecs.ByIdWithRelations(dto.Id);
            var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Table price with ID {dto.Id} not found.");

            // Validate duplicates
            var duplicates = dto.Items
                .GroupBy(i => i.TablePriceItemId)
                .Where(g => g.Count() > 1)
                .ToList();

            if ( duplicates.Any ( ) )
                throw new BusinessRuleException ( $"Duplicate items detected with ID(s): {string.Join ( ", ", duplicates.Select ( d => d.Key ) )}" );

            // Update core properties
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status;

            // Reassociate items
            existing.Items.Clear ( );

            foreach ( var itemDto in dto.Items )
            {
                if ( itemDto.TablePriceItemId <= 0 )
                    throw new CustomValidationException ( $"Invalid item ID in table price: {itemDto.TablePriceItemId}" );

                var item = await _itemRepository.GetEntityWithSpec(TablePriceItemSpecs.ByIdWithRelations(itemDto.TablePriceItemId))
                    ?? throw new NotFoundException($"Item with ID {itemDto.TablePriceItemId} not found.");

                existing.Items.Add ( item );
            }

            var updated = await _repository.UpdateAsync(dto.Id, existing)
                ?? throw new BusinessRuleException("Failed to update table price.");

            return updated;
        }
    }
}
