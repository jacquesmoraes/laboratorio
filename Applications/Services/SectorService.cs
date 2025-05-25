using Applications.Contracts;
using Applications.Dtos.Sector;
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
            var spec = SectorSpecification.SectorSpecs.ById(dto.SectorId);
            var existing = await _repository.GetEntityWithSpec(spec);
            if (existing == null) return null;

            existing.Name = dto.Name;
            return await _repository.UpdateAsync(dto.SectorId, existing);
        }
    }
}
