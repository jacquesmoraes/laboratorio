namespace API.Controllers
{
    [ApiController]
    [Route ( "auth" )]
    public class AuthController ( IIdentityService identityService ) : ControllerBase
    {
        private readonly IIdentityService _identityService = identityService;

        

        [HttpPost ( "register-admin" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> RegisterAdmin ( [FromBody] RegisterAdminUserDto dto )
        {
            var result = await _identityService.RegisterAdminUserAsync(dto);
            return Ok ( result );
        }

        [HttpPost ( "register-client" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> RegisterClient ( [FromBody] RegisterClientUserDto dto )
        {
            var result = await _identityService.RegisterClientUserAsync(dto);
            return Ok ( result );
        }

       

        [HttpPost ( "login" )]
        public async Task<IActionResult> Login ( [FromBody] LoginDto dto )
        {
            var result = await _identityService.LoginAsync(dto);
            return Ok ( result );
        }

    

       

        [HttpPost ( "complete-first-access" )]
        public async Task<IActionResult> CompleteFirstAccess ( [FromBody] FirstAccessPasswordResetDto dto )
        {
            var result = await _identityService.CompleteFirstAccessAsync(dto);
            return Ok ( result );
        }

        [HttpPost ( "resend-access-code/{clientId}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> ResendAccessCodeByClientId ( [FromRoute] int clientId )
        {
            var newCode = await _identityService.RegenerateAccessCodeByClientIdAsync(clientId);
            return Ok ( new { accessCode = newCode } );
        }


    }
}
