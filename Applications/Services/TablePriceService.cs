using Applications.Contracts;
using Applications.Dtos.Pricing;
using Core.Exceptions;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Pricing;

namespace Applications.Services
{
    public class TablePriceService ( IGenericRepository<TablePrice> repository, IGenericRepository<TablePriceItem> itemRepository ) : GenericService<TablePrice> ( repository ), ITablePriceService
    {
        private readonly IGenericRepository<TablePrice> _repository = repository;
        private readonly IGenericRepository<TablePriceItem> _itemRepository = itemRepository;

        public async Task<TablePrice> CreateFromDtoAsync ( CreateTablePriceDto dto )
        {
            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "O nome da tabela de preço é obrigatório." );

            if ( dto.Items == null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "A tabela de preço deve conter pelo menos um item." );

            var duplicates = dto.Items
        .GroupBy(i => i.TablePriceItemId)
        .Where(g => g.Count() > 1)
        .ToList();

            if ( duplicates.Any ( ) )
                throw new BusinessRuleException ( $"Itens duplicados detectados com ID(s): {string.Join ( ", ", duplicates.Select ( d => d.Key ) )}" );

            var itemIds = dto.Items.Select(i => i.TablePriceItemId).ToList();

            // Usar Specification para buscar os itens com base em seus IDs
            var spec = TablePriceItemSpecs.ByIds(itemIds); // crie essa spec abaixo
            var existingItems = await _itemRepository.GetAllAsync(spec);

            if ( existingItems.Count != itemIds.Count )
                throw new NotFoundException ( "Um ou mais TablePriceItems não foram encontrados." );

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
                throw new CustomValidationException ( "ID da tabela de preço inválido." );

            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "O nome da tabela de preço é obrigatório." );

            if ( dto.Items == null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "A tabela de preço deve conter pelo menos um item." );

            var spec = TablePriceSpecs.ByIdWithRelations(dto.Id);
            var existing = await _repository.GetEntityWithSpec(spec)
        ?? throw new NotFoundException($"Tabela de preço com ID {dto.Id} não encontrada.");

            // Validar duplicações
            var duplicates = dto.Items.GroupBy(i => i.TablePriceItemId).Where(g => g.Count() > 1).ToList();
            if ( duplicates.Any ( ) )
                throw new BusinessRuleException ( $"Itens duplicados detectados com ID(s): {string.Join ( ", ", duplicates.Select ( d => d.Key ) )}" );

            // Atualiza propriedades principais
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status;

            // Reassociar itens existentes
            existing.Items.Clear ( );

            foreach ( var itemDto in dto.Items )
            {
                if ( itemDto.TablePriceItemId <= 0 )
                    throw new CustomValidationException ( $"ID inválido para item da tabela de preço: {itemDto.TablePriceItemId}" );

                var item = await _itemRepository.GetEntityWithSpec(TablePriceItemSpecs.ByIdWithRelations(itemDto.TablePriceItemId))
    ?? throw new NotFoundException($"Item de ID {itemDto.TablePriceItemId} não encontrado.");


                existing.Items.Add ( item );
            }

            var updated = await _repository.UpdateAsync(dto.Id, existing)
        ?? throw new BusinessRuleException("Falha ao atualizar a tabela de preço.");

            return updated;
        }


    }


}
