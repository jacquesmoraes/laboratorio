namespace API.Controllers.Works
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class WorkSectionsController ( IMapper mapper, IGenericService<WorkSection> sectionService ) : BaseApiController
    {
        private readonly IGenericService<WorkSection> _sectionService = sectionService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = new BaseSpecification<WorkSection>();
            var sections = await _sectionService.GetAllWithSpecAsync(spec);
            var response = _mapper.Map<List<WorkSectionRecord>>(sections);
            return Ok ( response );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = new WorkSectionSpecification(id);
            var section = await _sectionService.GetEntityWithSpecAsync(spec);
            if ( section == null ) return NotFound ( );

            var response = _mapper.Map<WorkSectionRecord>(section);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateWorkSectionDto dto )
        {
            var entity = _mapper.Map<WorkSection>(dto);
            var created = await _sectionService.CreateAsync(entity);
            var response = _mapper.Map<WorkSectionRecord>(created);
            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, response );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateWorkSectionDto dto )
        {
            var spec = new WorkSectionSpecification(id);
            var existing = await _sectionService.GetEntityWithSpecAsync(spec);
            if ( existing == null ) return NotFound ( );

            _mapper.Map ( dto, existing );
            var updated = await _sectionService.UpdateAsync(id, existing);
            var response = _mapper.Map<WorkSectionRecord>(updated);
            return Ok ( response );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _sectionService.DeleteAsync(id);
            return deleted == null ? NotFound ( ) : NoContent ( );
        }
    }
}