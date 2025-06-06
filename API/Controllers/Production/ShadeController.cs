using Applications.Contracts;
using Applications.Dtos.Production;
using Applications.Records.Production;
using AutoMapper;
using Core.FactorySpecifications.ProductionSpecifications;
using Core.Models.Production;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShadeController(IShadeService service, IMapper mapper) : BaseApiController
    {
        private readonly IShadeService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? scaleId)
        {
            var spec = scaleId.HasValue
                ? ShadeSpecifications.ByScaleId(scaleId.Value)
                : ShadeSpecifications.All();

            var shades = await _service.GetAllWithSpecAsync(spec);
            return Ok(_mapper.Map<List<ShadeRecord>>(shades));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = ShadeSpecifications.ById(id);
            var shade = await _service.GetEntityWithSpecAsync(spec);
            return shade == null
                ? NotFound()
                : Ok(_mapper.Map<ShadeRecord>(shade));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShadeDto dto)
        {
            var created = await _service.CreateWithValidationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ShadeRecord>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateShadeDto dto)
        {
            var updated = await _service.UpdateWithValidationAsync(id, dto);
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
