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

        [HttpPost ( "change-password" )]
        [Authorize]
        public async Task<IActionResult> ChangePassword ( [FromBody] ChangePasswordDto dto )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Token inválido.");

            await _identityService.ChangePasswordAsync ( userId, dto );


            return Ok ( new { message = "Senha alterada com sucesso." } );
        }




        [HttpGet ( "client-users" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> GetClientUsers ( [FromQuery] QueryParams parameters )
        {
            var result = await _identityService.GetClientUsersPaginatedAsync(parameters);
            return Ok ( result );
        }

        // Mudar de userId para clientId
        [HttpGet ( "client-users/{userId}" )]
        public async Task<IActionResult> GetClientUserDetailsByUserId ( [FromRoute] string userId )
        {
            var result = await _identityService.GetClientUserDetailsByUserIdAsync(userId);
            return Ok ( result );
        }


        [HttpPut ( "client-users/{userId}/block" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> BlockClientUser ( [FromRoute] string userId )
        {
            await _identityService.BlockClientUserAsync ( userId );
            return Ok ( new { message = "Usuário bloqueado com sucesso." } );
        }

        [HttpPut ( "client-users/{userId}/unblock" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> UnblockClientUser ( [FromRoute] string userId )
        {
            await _identityService.UnblockClientUserAsync ( userId );
            return Ok ( new { message = "Usuário desbloqueado com sucesso." } );
        }

        [HttpPost ( "client-users/{userId}/reset-access-code" )]
        [Authorize ( Roles = "admin" )]
        public async Task<IActionResult> ResetClientUserAccessCode ( [FromRoute] string userId )
        {
            var newCode = await _identityService.RegenerateAccessCodeByUserIdAsync(userId);
            return Ok ( new { accessCode = newCode } );
        }



    }
}
