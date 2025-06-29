namespace API.Controllers.Sectors
{

    [Route ( "api/[controller]" )]
    [ApiController]
    public class SectorsController ( IMapper mapper, ISectorService sectorService ) : BaseApiController
    {
        private readonly ISectorService _sectorService = sectorService;
        private readonly IMapper _mapper = mapper;


        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = SectorSpecification.SectorSpecs.All();
            var sectors = await _sectorService.GetAllWithSpecAsync(spec);
            if ( sectors == null || !sectors.Any ( ) ) return NotFound ( );

            var response = _mapper.Map<List<SectorRecord>>(sectors);
            return Ok ( response );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = SectorSpecification.SectorSpecs.ById(id);
            var sector = await _sectorService.GetEntityWithSpecAsync(spec);
            if ( sector == null ) return NotFound ( );

            var response = _mapper.Map<SectorRecord>(sector);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateSectorDto dto )
        {
            var entity = _mapper.Map<Sector>(dto);
            var created = await _sectorService.CreateAsync(entity);
            var response = _mapper.Map<SectorRecord>(created);
            return CreatedAtAction ( nameof ( GetById ), new { id = response.SectorId }, response );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateSectorDto dto )
        {
            if ( id != dto.SectorId )
                return BadRequest ( $"Mismatch between route ID {id} and body ID {dto.SectorId}." );

            var updated = await _sectorService.UpdateFromDtoAsync(dto);
            if ( updated == null )
                return NotFound ( $"Sector with id {id} not found." );

            var response = _mapper.Map<SectorRecord>(updated);
            return Ok ( response );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _sectorService.DeleteAsync(id);
            return deleted == null ? NotFound ( ) : NoContent ( );
        }
    }
}
