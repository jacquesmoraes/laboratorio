using Applications.Identity;
using FluentAssertions;
using System.Net.Http.Json;
using Tests.Integration.Infrastructure;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests ( CustomWebApplicationFactory<Program> factory )
    {
        _client = factory.CreateClient ( );
    }

    [Fact]
    public async Task ShouldLoginWithSeededUser ( )
    {
        var loginDto = new LoginDto
        {
            Email = "admin@sistema.com",
            Password = "Pa$$w0rd"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine ( $"[DEBUG] Status: {response.StatusCode}" );
        Console.WriteLine ( $"[DEBUG] Content: {content}" );
        response.EnsureSuccessStatusCode ( );

        var result = await response.Content.ReadFromJsonAsync<AuthResponseRecord>();
        result.Should ( ).NotBeNull ( );
        result!.Token.Should ( ).NotBeNullOrWhiteSpace ( );
        result.User.Should ( ).NotBeNull ( );
        result.User.Role.Should ( ).Be ( "admin" );

    }

    [Fact]
    public async Task ShouldRejectInvalidCredentials ( )
    {
        var loginDto = new LoginDto
        {
            Email = "admin@sistema.com",
            Password = "WrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        response.StatusCode.Should ( ).Be ( System.Net.HttpStatusCode.Unauthorized );
    }

    [Fact]
    public async Task ShouldRejectNonExistentUser ( )
    {
        var loginDto = new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "AnyPassword"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        response.StatusCode.Should ( ).Be ( System.Net.HttpStatusCode.Unauthorized );
    }

    [Fact]
    public async Task Debug_CheckUserCreation()
    {
        // Este teste é apenas para debug
        var loginDto = new LoginDto
        {
            Email = "admin@sistema.com",
            Password = "Pa$$w0rd"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"[DEBUG] Status: {response.StatusCode}");
        Console.WriteLine($"[DEBUG] Content: {content}");
        
        // Não vamos falhar o teste, apenas mostrar o que está acontecendo
        Assert.True(true, "Debug test completed");
    }
}
