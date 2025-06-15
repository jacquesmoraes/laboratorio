using Applications.Contracts;
using Applications.Dtos.Production;
using Applications.Records.Production;
using AutoMapper;
using Core.FactorySpecifications.ProductionSpecifications;
using Core.Models.Production;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Production
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScaleController(IGenericService<Scale> scaleService, IMapper mapper) : BaseApiController
    {
        private readonly IGenericService<Scale> _scaleService = scaleService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _scaleService.GetAllWithSpecAsync(ScaleSpecifications.All());
            return Ok(_mapper.Map<IEnumerable<ScaleRecord>>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _scaleService.GetEntityWithSpecAsync(ScaleSpecifications.ById(id));
            return entity == null ? NotFound() : Ok(_mapper.Map<ScaleRecord>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScaleDto dto)
        {
            var entity = _mapper.Map<Scale>(dto);
            var created = await _scaleService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ScaleRecord>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateScaleDto dto)
        {
            var existing = await _scaleService.GetEntityWithSpecAsync(ScaleSpecifications.ById(id));
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            var updated = await _scaleService.UpdateAsync(id, existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _scaleService.DeleteAsync(id);
            return deleted == null ? NotFound() : NoContent();
        }
    }
}
