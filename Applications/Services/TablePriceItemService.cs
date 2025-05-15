using Applications.Dtos.Pricing;
using Applications.Interfaces;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Pricing;

namespace Applications.Services
{
    public class TablePriceItemService ( IGenericRepository<TablePriceItem> repository ) : GenericService<TablePriceItem> ( repository ), ITablePriceItemService
    {
        private readonly IGenericRepository<TablePriceItem> _repository = repository;

        public async Task<TablePriceItem?> UpdateFromDtoAsync ( int id, UpdateTablePriceItemDto dto )
        {
            var spec = TablePriceItemSpecs.ByIdWithRelations(id);
            var existing = await _repository.GetByIdAsync(id, spec);
            if ( existing == null ) return null;

            // Atualiza os campos
            existing.WorkTypeId = dto.WorkTypeId;
            existing.TablePriceId = dto.TablePriceId;
            existing.Price = dto.Price;

            // Passa o id e a entidade para o método Update
            var updated = await _repository.UpdateAsync(id, existing);
            return updated;
        }


    }

}
