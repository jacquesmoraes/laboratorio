using Applications.Contracts.Identity;
using Applications.Dtos.Identity;
using Applications.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController ( IIdentityService identityService ) : ControllerBase
    {
        private readonly IIdentityService _identityService = identityService;

        // ---------------------------
        // REGISTRO
        // ---------------------------

        [HttpPost ("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminUserDto dto)
        {
            var result = await _identityService.RegisterAdminUserAsync(dto);
            return Ok(result);
        }

        [HttpPost("register-client")]
        public async Task<IActionResult> RegisterClient([FromBody] RegisterClientUserDto dto)
        {
            var result = await _identityService.RegisterClientUserAsync(dto);
            return Ok(result);
        }

        // ---------------------------
        // LOGIN
        // ---------------------------

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _identityService.LoginAsync(dto);
            return Ok(result);
        }

        // ---------------------------
        // PRIMEIRO ACESSO
        // ---------------------------

        [HttpPost("validate-access-code")]
        public async Task<IActionResult> ValidateAccessCode([FromBody] ValidateAccessCodeDto dto)
        {
            var isValid = await _identityService.ValidateAccessCodeAsync(dto);
            return Ok(new { isValid });
        }

        [HttpPost("complete-first-access")]
        public async Task<IActionResult> CompleteFirstAccess([FromBody] FirstAccessPasswordResetDto dto)
        {
            var result = await _identityService.CompleteFirstAccessAsync(dto);
            return Ok(result);
        }
    }
}
