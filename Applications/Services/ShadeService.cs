using Applications.Contracts;
using Applications.Dtos.Production;
using AutoMapper;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Production;
using Core.Specifications;

namespace Applications.Services
{
    public class ShadeService(
        IGenericRepository<Shade> shadeRepo,
        IGenericRepository<Scale> scaleRepo,
        IMapper mapper,
        IUnitOfWork uow)
        : GenericService<Shade>(shadeRepo), IShadeService
    {
        private readonly IGenericRepository<Shade> _shadeRepo = shadeRepo;
        private readonly IGenericRepository<Scale> _scaleRepo = scaleRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _uow = uow;

        public async Task<Shade> CreateShade(CreateShadeDto dto)
        {
            var scale = await _scaleRepo.GetByIdAsync(dto.ScaleId, new BaseSpecification<Scale>());
            if (scale == null)
                throw new NotFoundException($"Escala de cor {dto.ScaleId} não encontrada.");

            var entity = _mapper.Map<Shade>(dto);
            return await base.CreateAsync ( entity );
        }

        public async Task<Shade?> UpdateWithValidationAsync(int id, CreateShadeDto dto)
        {
            var existing = await _shadeRepo.GetByIdAsync(id, new BaseSpecification<Shade>());
            if (existing == null)
                throw new NotFoundException($"Cor {dto.ScaleId} não encontrada.");

            var scale = await _scaleRepo.GetByIdAsync(dto.ScaleId, new BaseSpecification<Scale>());
            if (scale == null)
                throw new NotFoundException($"Escala de cor {dto.ScaleId} não encontrada.");

            existing.color = dto.Color ?? string.Empty;
            existing.ScaleId = dto.ScaleId;

            await _shadeRepo.UpdateAsync(id, existing);
            await _uow.SaveChangesAsync();
            return existing;
        }
    }
}
