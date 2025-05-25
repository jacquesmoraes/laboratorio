using Applications.Contracts;
using Applications.Dtos.Pricing;
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
            var spec = TablePriceSpecs.ByIdWithRelations(dto.Id);
            var existing = await _repository.GetEntityWithSpec(spec);
            if ( existing == null ) return null;

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

            return await _repository.UpdateAsync ( dto.Id, existing );
        }

    }


}
