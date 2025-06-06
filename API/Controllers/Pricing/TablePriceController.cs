using Applications.Contracts;
using Applications.Dtos.Pricing;
using Applications.Projections.Pricing;
using AutoMapper;
using Core.FactorySpecifications;
using Core.Models.Pricing;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Pricing
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablePriceController(IMapper mapper, ITablePriceService tablePriceService)
        : BaseApiController
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITablePriceService _tablePriceService = tablePriceService;

        /// <summary>
        /// Retorna todas as tabelas de preço com seus itens e clientes vinculados.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spec = TablePriceSpecs.AllWithRelations();
            var result = await _tablePriceService.GetAllWithSpecAsync(spec);

            var response = _mapper.Map<List<TablePriceResponseProjection>>(result);
            return Ok(response);
        }

        /// <summary>
        /// Retorna uma tabela de preço específica com seus itens e clientes vinculados.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spec = TablePriceSpecs.ByIdWithRelations(id);
            var result = await _tablePriceService.GetEntityWithSpecAsync(spec);
            if (result == null) return NotFound();

            var response = _mapper.Map<TablePriceResponseProjection>(result);
            return Ok(response);
        }

        /// <summary>
        /// Cria uma nova tabela de preço.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTablePriceDto dto)
        {
            var entity = _mapper.Map<TablePrice>(dto);
            foreach (var item in entity.Items)
                item.TablePrice = entity;

            var created = await _tablePriceService.CreateAsync(entity);
            var response = _mapper.Map<TablePriceResponseProjection>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        /// <summary>
        /// Atualiza uma tabela de preço existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTablePriceDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID inconsistente");

            var updated = await _tablePriceService.UpdateFromDtoAsync(dto);
            if (updated == null) return NotFound();

            var response = _mapper.Map<TablePriceResponseProjection>(updated);
            return Ok(response);
        }

        /// <summary>
        /// Remove uma tabela de preço.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tablePriceService.DeleteAsync(id);
            return result == null ? NotFound() : NoContent();
        }
    }
}
