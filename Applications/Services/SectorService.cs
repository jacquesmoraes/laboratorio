using Applications.Contracts;
using Applications.Dtos.Sector;
using Core.Exceptions;
using Core.FactorySpecifications.SectorSpecifications;
using Core.Interfaces;
using Core.Models.ServiceOrders;

namespace Applications.Services
{
    public class SectorService(IGenericRepository<Sector> repository)
        : GenericService<Sector>(repository), ISectorService
    {
        private readonly IGenericRepository<Sector> _repository = repository;

        public async Task<Sector?> UpdateFromDtoAsync(UpdateSectorDto dto)
        {
             if (dto.SectorId <= 0)
                throw new CustomValidationException ("ID do setor inválido.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new CustomValidationException ("O nome do setor é obrigatório.");

            var spec = SectorSpecification.SectorSpecs.ById(dto.SectorId);
           var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Setor com ID {dto.SectorId} não encontrado.");
            
            if (existing.Name != dto.Name)
            {
                // Verifica se já existe outro setor com o mesmo nome
                var nameSpec = SectorSpecification.SectorSpecs.ByName(dto.Name);
                var sectorWithSameName = await _repository.GetEntityWithSpec(nameSpec);
                
                if (sectorWithSameName != null && sectorWithSameName.SectorId != dto.SectorId)
                    throw new BusinessRuleException($"Já existe um setor com o nome '{dto.Name}'.");
            }

            existing.Name = dto.Name;
             var updated = await _repository.UpdateAsync(dto.SectorId, existing)
                ?? throw new BusinessRuleException("Falha ao atualizar o setor.");

            return updated;
        }
    }
}
