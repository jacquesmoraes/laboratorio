using Applications.Contracts;
using Applications.Dtos.Production;
using AutoMapper;
using Core.FactorySpecifications.ProductionSpecifications;
using Core.Models.Production;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShadeController : BaseApiController
     {
        private readonly IGenericService<Shade> _service;
        private readonly IMapper _mapper;

        public ShadeController(IGenericService<Shade> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? scaleId)
        {
            var spec = scaleId.HasValue
                ? ShadeSpecifications.ByScaleId(scaleId.Value)
                : ShadeSpecifications.All();

            var shades = await _service.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<List<ShadeDto>>(shades));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = ShadeSpecifications.ById(id);
            var shade = await _service.GetEntityWithSpecAsync(spec);
            return shade == null ? NotFound() : Ok(_mapper.Map<ShadeDto>(shade));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShadeDto dto)
        {
            var entity = _mapper.Map<Shade>(dto);
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ShadeDto>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateShadeDto dto)
        {
            var existing = await _service.GetEntityWithSpecAsync(ShadeSpecifications.ById(id));
            if (existing == null) return NotFound();

            existing.color = dto.Color;
            existing.ScaleId = dto.ScaleId;

            var updated = await _service.UpdateAsync(id, existing);
            return updated == null ? NotFound() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted == null ? NotFound() : NoContent();
        }
    }
}
