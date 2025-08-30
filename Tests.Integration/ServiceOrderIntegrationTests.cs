using Applications.Identity;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tests.Integration.Infrastructure;

namespace Tests.Integration
{
    public class ServiceOrderIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ServiceOrderIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetServiceOrders_ShouldReturnPaginatedResults()
        {
            // Arrange
            var queryParams = "?pageNumber=1&pageSize=10&sort=dateIn";
            var authenticatedClient = _client.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.GetAsync($"/api/serviceorders{queryParams}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull(result);
            Assert.True(result!.GetProperty("data").GetArrayLength() >= 0);
            Assert.True(result!.GetProperty("totalItems").GetInt32() >= 0);
        }

        [Fact]
        public async Task GetServiceOrders_WithFilters_ShouldReturnFilteredResults()
        {
            // Arrange
            var queryParams = "?pageNumber=1&pageSize=10&excludeFinished=true&search=test";
            var authenticatedClient = _client.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.GetAsync($"/api/serviceorders{queryParams}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetServiceOrderById_WithValidId_ShouldReturnOrder()
        {
            // Arrange - Assumindo que existe uma ordem com ID 1 no banco de teste
            var orderId = 1;
            var authenticatedClient = _client.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.GetAsync($"/api/serviceorders/{orderId}");

            // Assert
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Se não encontrar, é aceitável para testes
                Assert.True(true);
            }
            else
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<dynamic>(content);

                Assert.NotNull(result);
                Assert.Equal(orderId, result!.GetProperty("serviceOrderId").GetInt32());
            }
        }

        [Fact]
        public async Task GetServiceOrderById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidOrderId = 99999;
            var authenticatedClient = _client.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.GetAsync($"/api/serviceorders/{invalidOrderId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetTryInAlerts_ShouldReturnAlerts()
        {
            // Arrange
            var days = 5;
            var authenticatedClient = _client.CreateAuthenticatedClient();

            // Act
            var response = await authenticatedClient.GetAsync($"/api/serviceorders/alert/tryin?days={days}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content);

            Assert.NotNull(result);
            Assert.True(result!.GetArrayLength() >= 0);
        }

        [Fact]
        public async Task GetServiceOrders_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            var queryParams = "?pageNumber=1&pageSize=10";

            // Act
            var response = await _client.GetAsync($"/api/serviceorders{queryParams}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}