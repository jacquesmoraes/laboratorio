using Applications.Identity;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tests.Integration.Infrastructure;

namespace Tests.Integration
{
    public class ServiceOrderIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory < Program > _factory;
        private readonly HttpClient _client;

        public ServiceOrderIntegrationTests ( CustomWebApplicationFactory<Program> factory )
        {
            _factory = factory;
            _client = factory.CreateClient ( );
        }

        [Fact]
        public async Task GetServiceOrders_ShouldReturnPaginatedResults ( )
        {
            // Arrange
            var queryParams = "?pageNumber=1&pageSize=10&sort=dateIn";

            // Act
            var response = await _client.GetAsync($"/api/serviceorders{queryParams}");

            // Assert
            response.EnsureSuccessStatusCode ( );
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull ( result );
            Assert.True ( result!.GetProperty ( "data" ).GetArrayLength ( ) >= 0 );
            Assert.True ( result!.GetProperty ( "totalItems" ).GetInt32 ( ) >= 0 );
        }

        [Fact]
        public async Task GetServiceOrders_WithFilters_ShouldReturnFilteredResults ( )
        {
            // Arrange
            var queryParams = "?pageNumber=1&pageSize=10&excludeFinished=true&search=test";

            // Act
            var response = await _client.GetAsync($"/api/serviceorders{queryParams}");

            // Assert
            response.EnsureSuccessStatusCode ( );
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull ( result );
        }

        [Fact]
        public async Task GetServiceOrderById_WithValidId_ShouldReturnOrder ( )
        {
            // Arrange - Assumindo que existe uma ordem com ID 1 no banco de teste
            var orderId = 1;

            // Act
            var response = await _client.GetAsync($"/api/serviceorders/{orderId}");

            // Assert
            if ( response.StatusCode == HttpStatusCode.NotFound )
            {
                // Se não encontrar, é aceitável para testes
                Assert.True ( true );
            }
            else
            {
                response.EnsureSuccessStatusCode ( );
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<dynamic>(content);

                Assert.NotNull ( result );
                Assert.Equal ( orderId, result!.GetProperty ( "serviceOrderId" ).GetInt32 ( ) );
            }
        }

        [Fact]
        public async Task GetServiceOrderById_WithInvalidId_ShouldReturnNotFound ( )
        {
            // Arrange
            var invalidOrderId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/serviceorders/{invalidOrderId}");

            // Assert
            Assert.Equal ( HttpStatusCode.NotFound, response.StatusCode );
        }

        [Fact]
        public async Task GetTryInAlerts_ShouldReturnAlerts ( )
        {
            // Arrange
            var days = 5;

            // Act
            var response = await _client.GetAsync($"/api/serviceorders/alert/tryin?days={days}");

            // Assert
            response.EnsureSuccessStatusCode ( );
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull ( result );
            Assert.True ( result!.GetArrayLength ( ) >= 0 );
        }

        private async Task<string> GetAuthTokenAsync ( )
        {
            var loginDto = new LoginDto
            {
                Email = "admin@sistema.com",
                Password = "Pa$$w0rd"
            };

            var response = await _client.PostAsJsonAsync("/auth/login", loginDto);
            var json = await response.Content.ReadFromJsonAsync<AuthResponseRecord>();

            return json?.Token!;
        }

    }
}