using API.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace Tests.Integration;

public class ExceptionMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExceptionMiddlewareTests ( CustomWebApplicationFactory factory )
    {
        _client = factory.CreateClient ( );
    }

    [Theory]
    [InlineData ( "/test/badrequest", HttpStatusCode.BadRequest, "BadRequest test exception" )]
    [InlineData ( "/test/unauthorized", HttpStatusCode.Unauthorized, "Unauthorized access" )]
    [InlineData ( "/test/forbidden", HttpStatusCode.Forbidden, "Access denied" )]
    [InlineData ( "/test/notfound", HttpStatusCode.NotFound, "Resource not found" )]
    [InlineData ( "/test/conflict", HttpStatusCode.Conflict, "Conflict occurred" )]
    [InlineData ( "/test/unprocessable", HttpStatusCode.UnprocessableEntity, "Unprocessable entity" )]
    [InlineData ( "/test/invalid", HttpStatusCode.BadRequest, "Operação inválida" )]
    [InlineData ( "/test/unexpected", HttpStatusCode.InternalServerError, "Erro inesperado" )]


    public async Task Middleware_Should_Handle_Exception_Types_Correctly ( string url, HttpStatusCode expectedStatus, string expectedMessage )
    {
        // Act
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should ( ).Be ( expectedStatus );
        content.Should ( ).ContainEquivalentOf ( expectedMessage );

    }

    [Fact]
    public async Task Middleware_Should_Return_404_When_Endpoint_Not_Found ( )
    {
        var response = await _client.GetAsync("/rota-invalida-qualquer");

        response.StatusCode.Should ( ).Be ( HttpStatusCode.NotFound );

        var content = await response.Content.ReadAsStringAsync();
        content.Should ( ).Contain ( "Recurso não encontrado" );
    }

    [Fact]
    public async Task Middleware_Should_Return_401_When_Accessing_Protected_Endpoint_WithoutToken ( )
    {
        // Act
        var response = await _client.GetAsync("/test/auth-required");
        var content = await response.Content.ReadAsStringAsync();

        // Assert - status code
        response.StatusCode.Should ( ).Be ( HttpStatusCode.Unauthorized );

        // Desserializa a resposta JSON
        var result = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should ( ).NotBeNull ( );
        result!.StatusCode.Should ( ).Be ( 401 );

        // Valida a mensagem da resposta com flexibilidade
        Assert.True (
            result.Message.Contains ( "Não autorizado", StringComparison.OrdinalIgnoreCase ) ||
            result.Message.Contains ( "unauthorized", StringComparison.OrdinalIgnoreCase ),
            $"Mensagem esperada: 'Não autorizado' ou 'unauthorized'. Mensagem real: {result.Message}"
        );
    }

}
