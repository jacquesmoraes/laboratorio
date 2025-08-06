using API.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Tests.Integration.Infrastructure;

namespace Tests.Integration;

public class ExceptionMiddlewareTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExceptionMiddlewareTests ( CustomWebApplicationFactory<Program> factory )
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
    [InlineData ( "/test/invalid", HttpStatusCode.BadRequest, "Invalid operation" )]
    [InlineData ( "/test/unexpected", HttpStatusCode.InternalServerError, "Unexpected server error" )]
    public async Task Middleware_Should_Handle_Exception_Types_Correctly ( string url, HttpStatusCode expectedStatus, string expectedMessage )
    {
        // Act
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        // Assert - status code
        response.StatusCode.Should().Be(expectedStatus);

        // Assert - error message
        using var doc = JsonDocument.Parse(content);
        var message = doc.RootElement.GetProperty("message").GetString();

        message.Should().Contain(expectedMessage);
    }

    [Fact]
    public async Task Middleware_Should_Return_404_When_Endpoint_Not_Found ( )
    {
        var response = await _client.GetAsync("/rota-invalida-qualquer");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();

        // Deserialize JSON to avoid encoding issues
        var result = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Resource not found");
    }

    [Fact]
    public async Task Middleware_Should_Return_401_When_Accessing_Protected_Endpoint_WithoutToken ( )
    {
        // Act
        var response = await _client.GetAsync("/test/auth-required");
        var content = await response.Content.ReadAsStringAsync();

        // Assert - status code
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Deserialize JSON response
        var result = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(401);

        // Validate response message with flexibility
        Assert.True(
            result.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase),
            $"Expected message: 'unauthorized'. Actual message: {result.Message}"
        );
    }

    [Fact]
    public async Task Debug_404_Test ( )
    {
        var response = await _client.GetAsync("/rota-invalida-qualquer");

        var content = await response.Content.ReadAsStringAsync();

        // Debug: print status code and content
        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.WriteLine($"Content: {content}");

        // Accepting multiple statuses during debugging
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Unauthorized);
    }
}
