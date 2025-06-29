namespace API.Controllers.Production
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ShadeController ( IShadeService shadeService, IMapper mapper ) : BaseApiController
    {
        private readonly IShadeService _shadeService = shadeService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll ( [FromQuery] int? scaleId )
        {
            var spec = scaleId.HasValue
                ? ShadeSpecifications.ByScaleId(scaleId.Value)
                : ShadeSpecifications.All();

            var shades = await _shadeService.GetAllWithSpecAsync(spec);
            return Ok ( _mapper.Map<List<ShadeRecord>> ( shades ) );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = ShadeSpecifications.ById(id);
            var shade = await _shadeService.GetEntityWithSpecAsync(spec);
            return shade == null
                ? NotFound ( )
                : Ok ( _mapper.Map<ShadeRecord> ( shade ) );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] CreateShadeDto dto )
        {
            var created = await _shadeService.CreateShade(dto);
            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, _mapper.Map<ShadeRecord> ( created ) );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] CreateShadeDto dto )
        {
            var updated = await _shadeService.UpdateWithValidationAsync(id, dto);
            return updated == null ? NotFound ( ) : NoContent ( );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _shadeService.DeleteAsync(id);
            return deleted == null ? NotFound ( ) : NoContent ( );
        }
    }
}
