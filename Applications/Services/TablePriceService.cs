using Applications.Dtos.Pricing;
using Applications.Records.Pricing;
using Core.Exceptions;
using Core.FactorySpecifications.ClientsSpecifications;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.Pricing;
using Core.Models.Works;

namespace Applications.Services
{
    public class TablePriceService (
        IGenericRepository<TablePrice> repository,
        IGenericRepository<Client> clientRepo,
        IGenericRepository<WorkType> workTypeRepo
    ) : GenericService<TablePrice> ( repository ), ITablePriceService
    {
        private readonly IGenericRepository<TablePrice> _repository = repository;
        private readonly IGenericRepository<Client> _clientRepo = clientRepo;
        private readonly IGenericRepository<WorkType> _workTypeRepo = workTypeRepo;

        public async Task<TablePrice> CreateFromDtoAsync ( CreateTablePriceDto dto )
        {
            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "Table price name is required." );

            if ( dto.Items is null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "The table price must contain at least one item." );

            // ✅ Verificação de duplicatas por tipo de serviço
            var duplicates = dto.Items
        .GroupBy(i => i.WorkTypeId)
        .Where(g => g.Count() > 1)
        .ToList();

            if ( duplicates.Any ( ) )
                throw new BusinessRuleException (
                    $"Duplicate work types detected: {string.Join ( ", ", duplicates.Select ( d => d.Key ) )}" );

            var workTypeIds = dto.Items.Select(i => i.WorkTypeId).Distinct().ToList();
            var spec = WorkTypeSpecs.ByIds(workTypeIds);
            var existingWorkTypes = await _workTypeRepo.GetAllAsync(spec);

            if ( existingWorkTypes.Count != workTypeIds.Count )
                throw new NotFoundException ( "One or more work types were not found." );

            var items = dto.Items.Select(i =>
    {
        var workType = existingWorkTypes.First(w => w.Id == i.WorkTypeId);
        return new TablePriceItem
        {
            WorkTypeId = workType.Id,
            WorkType = workType,
            Price = i.Price
        };
    }).ToList();

            var tablePrice = new TablePrice
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = false,
                Items = items
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

            if ( dto.Items is null || !dto.Items.Any ( ) )
                throw new CustomValidationException ( "The table price must contain at least one item." );

            var spec = TablePriceSpecs.ByIdWithRelations(dto.Id);
            var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Table price with ID {dto.Id} not found.");

            var workTypeIds = dto.Items.Select(i => i.WorkTypeId).Distinct().ToList();
            var workTypeSpec = WorkTypeSpecs.ByIds(workTypeIds);
            var existingWorkTypes = await _workTypeRepo.GetAllAsync(workTypeSpec);

            if ( existingWorkTypes.Count != workTypeIds.Count )
                throw new NotFoundException ( "One or more work types were not found." );

            existing.Items.Clear ( );

            foreach ( var itemDto in dto.Items )
            {
                var workType = existingWorkTypes.First(w => w.Id == itemDto.WorkTypeId);
                existing.Items.Add ( new TablePriceItem
                {
                    WorkTypeId = workType.Id,
                    WorkType = workType,
                    Price = itemDto.Price
                } );
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Status = dto.Status;

            var updated = await _repository.UpdateAsync(dto.Id, existing)
                ?? throw new BusinessRuleException("Failed to update table price.");

            return updated;
        }

        public async Task<TablePriceItemRecord?> GetItemPriceByClientAndWorkTypeAsync ( int clientId, int workTypeId )
        {
            var spec = ClientSpecification.ClientSpecs.ByIdWithTablePriceItems(clientId);
            var client = await _clientRepo.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Cliente {clientId} não encontrado.");

            if ( client.TablePrice is null )
                throw new BusinessRuleException ( "Cliente não possui tabela de preço associada." );

            var item = client.TablePrice.Items.FirstOrDefault(i => i.WorkTypeId == workTypeId);

            if ( item is null )
                return null;

            return new TablePriceItemRecord
            {
                WorkTypeId = item.WorkTypeId,
                WorkTypeName = item.WorkType.Name,
                Price = item.Price
            };
        }
    }
}
