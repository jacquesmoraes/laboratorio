using Applications.Contracts;
using Applications.Records.Clients;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Clients
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClientBalanceController ( IMapper mapper, IClientBalanceService balanceService )
        : BaseApiController
    {
        private readonly IClientBalanceService _balanceService = balanceService;
        private readonly IMapper _mapper = mapper;
        [HttpGet ( "{id}/balance" )]
        public async Task<IActionResult> GetBalance ( int id )
        {
            var balance = await _balanceService.GetClientBalanceAsync(id);

            var response = _mapper.Map<ClientBalanceRecord>(balance);

            return Ok ( response );
        }
    }
}

