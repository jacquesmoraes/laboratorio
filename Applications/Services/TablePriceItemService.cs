using Applications.Contracts;
using Applications.Dtos.Pricing;
using Core.Exceptions;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Pricing;

namespace Applications.Services
{
    public class TablePriceItemService ( IGenericRepository<TablePriceItem> repository ) : GenericService<TablePriceItem> ( repository ), ITablePriceItemService
    {
        private readonly IGenericRepository<TablePriceItem> _repository = repository;

        public async Task<TablePriceItem?> UpdateFromDtoAsync(int id, UpdateTablePriceItemDto dto)
    {
        if (id <= 0)
            throw new CustomValidationException("ID do item da tabela de preço inválido.");

        if (string.IsNullOrWhiteSpace(dto.ItemName))
            throw new CustomValidationException("O nome do item é obrigatório.");

        if (dto.Price < 0)
            throw new CustomValidationException("O preço não pode ser negativo.");

        var existing = await _repository.GetEntityWithSpec(TablePriceItemSpecs.ByIdWithRelations(id))
            ?? throw new NotFoundException($"Item da tabela de preço com ID {id} não encontrado.");

        
            existing.ItemName = dto.ItemName;
        existing.Price = dto.Price;

        var updated = await _repository.UpdateAsync(id, existing)
            ?? throw new BusinessRuleException("Falha ao atualizar o item da tabela de preço.");

        return updated;
    }


    }

}
