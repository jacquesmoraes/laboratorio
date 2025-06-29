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
        /// Returns all price tables with their associated items and clients.
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
        /// Returns a specific price table with its associated items and clients.
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
        /// Creates a new price table.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTablePriceDto dto)
        {
            var created = await _tablePriceService.CreateFromDtoAsync(dto);
            var response = _mapper.Map<TablePriceResponseProjection>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        /// <summary>
        /// Updates an existing price table.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTablePriceDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Inconsistent ID.");

            var updated = await _tablePriceService.UpdateFromDtoAsync(dto);
            if (updated == null) return NotFound();

            var response = _mapper.Map<TablePriceResponseProjection>(updated);
            return Ok(response);
        }

        /// <summary>
        /// Deletes a price table.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tablePriceService.DeleteAsync(id);
            return result == null ? NotFound() : NoContent();
        }
    }
}
