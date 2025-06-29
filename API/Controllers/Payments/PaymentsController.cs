namespace API.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(
        IPaymentService clientPaymentService,
        IMapper mapper) : BaseApiController
    {
        private readonly IPaymentService _clientPaymentService = clientPaymentService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Creates a new payment for a client.
        /// </summary>
        [HttpPost("client")]
        public async Task<IActionResult> CreateClientPayment([FromBody] CreatePaymentDto dto)
        {
            var payment = await _clientPaymentService.RegisterClientPaymentAsync(dto);
            var response = _mapper.Map<ClientPaymentRecord>(payment);
            return CreatedAtAction(nameof(GetClientPaymentById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Returns a paginated list of payments with filters and sorting.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Pagination<ClientPaymentRecord>>> GetAll([FromQuery] PaymentParams parameters)
        {
            var result = await _clientPaymentService.GetPaginatedAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Gets a client payment by its ID.
        /// </summary>
        [HttpGet("client/{id}")]
        public async Task<IActionResult> GetClientPaymentById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid payment ID.");

            var spec = PaymentSpecification.PaymentSpecs.ById(id);
            var payment = await _clientPaymentService.GetEntityWithSpecAsync(spec);

            if (payment == null)
                return NotFound($"Payment with ID {id} not found.");

            var dto = _mapper.Map<ClientPaymentRecord>(payment);
            return Ok(dto);
        }
    }
}
