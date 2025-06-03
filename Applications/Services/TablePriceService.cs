using Applications.Contracts;
using Applications.Dtos.Pricing;
using Core.Exceptions;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Pricing;

namespace Applications.Services
{
    public class TablePriceService ( IGenericRepository<TablePrice> repository ) : GenericService<TablePrice> ( repository ), ITablePriceService
    {
        private readonly IGenericRepository<TablePrice> _repository = repository;


        public async Task<TablePrice?> UpdateFromDtoAsync ( UpdateTablePriceDto dto )
        {
            if (dto.Id <= 0)
                throw new CustomValidationException ("ID da tabela de preço inválido.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new CustomValidationException ("O nome da tabela de preço é obrigatório.");

            var spec = TablePriceSpecs.ByIdWithRelations(dto.Id);
            var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Tabela de preço com ID {dto.Id} não encontrada.");
            if (dto.Items == null || !dto.Items.Any())
                throw new CustomValidationException ("A tabela de preço deve conter pelo menos um item.");

             foreach (var item in dto.Items)
            {
                if (item.WorkTypeId <= 0)
                    throw new CustomValidationException ($"ID do tipo de trabalho inválido para o item {item.Id}.");

                if (item.Price < 0)
                    throw new CustomValidationException ($"O preço não pode ser negativo para o item {item.Id}.");

                // Check for duplicate work types
                if (dto.Items.Count(x => x.WorkTypeId == item.WorkTypeId) > 1)
                    throw new BusinessRuleException($"O tipo de trabalho {item.WorkTypeId} está duplicado na tabela.");
            }

            // Atualiza dados principais
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status;

            // Zera e recria os itens
            existing.Items.Clear ( );
            foreach ( var itemDto in dto.Items )
            {
                existing.Items.Add ( new TablePriceItem
                {
                  WorkTypeId = itemDto.WorkTypeId,  
                    Price = itemDto.Price,
                    TablePriceId = existing.Id
                } );
            }

            var updated = await _repository.UpdateAsync(dto.Id, existing)
                ?? throw new BusinessRuleException("Falha ao atualizar a tabela de preço.");

            return updated;
        }

    }


}
