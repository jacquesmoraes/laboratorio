namespace API.Controllers.Works
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class WorkTypesController ( IMapper mapper, IWorkTypeService workTypeService ) : BaseApiController
    {
        private readonly IMapper _mapper = mapper;
        private readonly IWorkTypeService _workTypeService = workTypeService;

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var types = await _workTypeService.GetAllWithSectionsAsync();
            var response = _mapper.Map<List<WorkTypeResponseRecord>>(types);
            return Ok ( response );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var workType = await _workTypeService.GetByIdWithSectionAsync(id);
            if ( workType == null ) return NotFound ( );

            var response = _mapper.Map<WorkTypeResponseRecord>(workType);
            return Ok ( response );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateWorkTypeDto dto )
        {
            var entity = _mapper.Map<WorkType>(dto);
            var created = await _workTypeService.CreateAsync(entity);
            var withSection = await _workTypeService.GetByIdWithSectionAsync(created.Id);

            var response = _mapper.Map<WorkTypeResponseRecord>(withSection);
            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, response );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] UpdateWorkTypeDto dto )
        {
            var existing = await _workTypeService.GetByIdWithSectionAsync(id);
            if ( existing == null ) return NotFound ( );

            _mapper.Map ( dto, existing );
            var updated = await _workTypeService.UpdateAsync(id, existing);

            var response = _mapper.Map<WorkTypeResponseRecord>(updated);
            return Ok ( response );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _workTypeService.DeleteAsync(id);
            if ( deleted == null ) return NotFound ( );

            return NoContent ( );
        }
    }
}
