using Applications.Contracts;
using Applications.Dtos.Pricing;
using Applications.Records.Pricing;
using AutoMapper;
using Core.FactorySpecifications;
using Core.Models.Pricing;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Pricing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablePriceItemController(IMapper mapper, ITablePriceItemService tablePriceItemService) : BaseApiController
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITablePriceItemService _tablePriceItemService = tablePriceItemService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = TablePriceItemSpecs.All();
            var items = await _tablePriceItemService.GetAllWithSpecAsync(spec);
            var response = _mapper.Map<IEnumerable<TablePriceItemsResponseRecord>>(items);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = TablePriceItemSpecs.ByIdWithRelations(id);
            var item = await _tablePriceItemService.GetEntityWithSpecAsync(spec);
            if (item == null)
                return NotFound();

            var response = _mapper.Map<TablePriceItemsResponseRecord>(item);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTablePriceItemDto dto)
        {
            var entity = _mapper.Map<TablePriceItem>(dto);
            var created = await _tablePriceItemService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.TablePriceItemId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTablePriceItemDto dto)
        {
            if (dto.Id != id)
                return BadRequest("ID in URL does not match ID in body.");

            var updated = await _tablePriceItemService.UpdateFromDtoAsync(id, dto);
            if (updated == null) return NotFound();

            var response = _mapper.Map<TablePriceItemsResponseRecord>(updated);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _tablePriceItemService.DeleteAsync(id);
            return deleted == null ? NotFound() : NoContent();
        }
    }
}
