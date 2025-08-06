using Applications.Identity;
using FluentAssertions;
using System.Net.Http.Json;
using Tests.Integration.Infrastructure;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShouldLoginWithSeededUser()
    {
        var loginDto = new LoginDto
        {
            Email = "admin@teste.com",
            Password = "Senha123!"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponseRecord>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
    }
}
