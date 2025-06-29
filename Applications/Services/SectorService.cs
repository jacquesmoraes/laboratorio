namespace Applications.Services
{
    public class SectorService ( IGenericRepository<Sector> repository )
        : GenericService<Sector> ( repository ), ISectorService
    {
        private readonly IGenericRepository<Sector> _repository = repository;

        public async Task<Sector?> UpdateFromDtoAsync ( UpdateSectorDto dto )
        {
            if ( dto.SectorId <= 0 )
                throw new CustomValidationException ( "Invalid sector ID." );

            if ( string.IsNullOrWhiteSpace ( dto.Name ) )
                throw new CustomValidationException ( "Sector name is required." );

            var spec = SectorSpecs.ById(dto.SectorId);
            var existing = await _repository.GetEntityWithSpec(spec)
                ?? throw new NotFoundException($"Sector with ID {dto.SectorId} not found.");

            if ( existing.Name != dto.Name )
            {
                // Check if another sector with the same name already exists
                var nameSpec = SectorSpecs.ByName(dto.Name);
                var sectorWithSameName = await _repository.GetEntityWithSpec(nameSpec);

                if ( sectorWithSameName != null && sectorWithSameName.SectorId != dto.SectorId )
                    throw new BusinessRuleException ( $"A sector with the name '{dto.Name}' already exists." );
            }

            existing.Name = dto.Name;

            var updated = await _repository.UpdateAsync(dto.SectorId, existing)
                ?? throw new BusinessRuleException("Failed to update sector.");

            return updated;
        }
    }
}
