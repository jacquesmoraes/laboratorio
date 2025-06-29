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

    }
}