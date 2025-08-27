namespace API.Controllers.Tests
{
    [ApiController]
    [Route ( "test" )]
    public class TestController : ControllerBase
    {
        [HttpGet ( "badrequest" )]
        public IActionResult ThrowBadRequest ( ) => throw new BadRequestException ( "BadRequest test exception" );

        [HttpGet ( "unauthorized" )]
        public IActionResult ThrowUnauthorized ( ) => throw new UnauthorizedAccessException ( "Unauthorized access" );

        [HttpGet ( "forbidden" )]
        public IActionResult ThrowForbidden ( ) => throw new ForbiddenException ( "Access denied" );

        [HttpGet ( "notfound" )]
        public IActionResult ThrowNotFound ( ) => throw new NotFoundException ( "Resource not found" );

        [HttpGet ( "conflict" )]
        public IActionResult ThrowConflict ( ) => throw new ConflictException ( "Conflict occurred" );

        [HttpGet ( "unprocessable" )]
        public IActionResult ThrowUnprocessable ( ) => throw new UnprocessableEntityException ( "Unprocessable entity" );

        [HttpGet ( "invalid" )]
        public IActionResult ThrowInvalidOperation ( ) => throw new InvalidOperationException ( "Operação inválida" );

        [HttpGet ( "unexpected" )]
        public IActionResult ThrowUnexpected ( ) => throw new Exception ( "Erro inesperado" );

        [HttpGet ( "auth-required" )]
        [Authorize] // ← obrigatório
        public IActionResult RequiresAuth ( ) => Ok ( "Sucesso" );

        //
        // Teste de envio de e-mail
        //
        [HttpPost ( "test-email" )]
        [AllowAnonymous]
        public async Task<IActionResult> SendTestEmail ( [FromServices] EmailService emailService )
        {
            await emailService.SendEmailAsync (
                "jacquesbarrosmoraes@gmail.com",
                "✅ Teste SMTP HostGator funcionando!",
                """
        <p>Este é um teste de envio via <strong>SMTP da HostGator (Titan)</strong>.</p>
        <p>Se você recebeu este e-mail, sua API no Docker está <strong>enviando e-mails com sucesso!</strong></p>
        <p>🚀</p>
        """
            );

            return Ok ( "E-mail enviado." );
        }



    }
}