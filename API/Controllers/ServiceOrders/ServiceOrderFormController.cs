namespace API.Controllers.ServiceOrders
{
    [ApiController]
    [Route ( "api/serviceorders/form" )]
    public class ServiceOrderFormController (
        IClientService clientService,
        ISectorService sectorService,
        IWorkTypeService workTypeService,
        IGenericService<Scale> scaleService,
        IShadeService shadeService,
        IMapper mapper )
        : BaseApiController
    {
        private readonly IClientService _clientService = clientService;
        private readonly ISectorService _sectorService = sectorService;
        private readonly IWorkTypeService _workTypeService = workTypeService;
        private readonly IGenericService<Scale> _scaleService = scaleService;
        private readonly IShadeService _shadeService = shadeService;
        private readonly IMapper _mapper = mapper;

        [HttpGet ( "basic-data" )]
        public async Task<IActionResult> GetFormBasicData ( )
        {
            var clients = await _clientService.GetAllWithSpecAsync(ClientSpecs.AllForForm());
            var sectors = await _sectorService.GetAllWithSpecAsync(new BaseSpecification<Sector>());

            var response = new
            {
                Clients = _mapper.Map<List<ClientResponseRecord>>(clients),
                Sectors = _mapper.Map<List<SectorRecord>>(sectors)
            };

            return Ok ( response );
        }

        [HttpGet ( "works-data" )]
        public async Task<IActionResult> GetFormWorksData ( )
        {
            var workTypes = await _workTypeService.GetAllForFormAsync(); 
            var scales = await _scaleService.GetAllWithSpecAsync(new BaseSpecification<Scale>());
            var shades = await _shadeService.GetAllWithSpecAsync(ShadeSpecs.AllWithScale());

            var response = new
            {
                WorkTypes = _mapper.Map<List<WorkTypeResponseRecord>>(workTypes),
                Scales = _mapper.Map<List<ScaleRecord>>(scales),
                Shades = _mapper.Map<List<ShadeRecord>>(shades)
            };

            return Ok ( response );
        }
    }
}