using Applications.Services;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Tests.Unit.Applications;

public class ClientAreaServiceTests
{
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock = new();
    private readonly Mock<IGenericRepository<BillingInvoice>> _invoiceRepoMock = new();
    private readonly Mock<IGenericRepository<Payment>> _paymentRepoMock = new();

    private readonly Mock<ILogger<ClientAreaService>> _loggerMock = new();

    private readonly ClientAreaService _service;

    public ClientAreaServiceTests ( )
    {


        _service = new ClientAreaService (
            _clientRepoMock.Object,
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
             _loggerMock.Object

             );
    }

    [Fact]
    public async Task GetClientBasicDashboardAsync_ShouldReturnCorrectBalanceInfo ( )
    {
        // Arrange
        var clientId = 1;
        var client = new Client
        {
            ClientId = clientId,
            ClientName = "Clínica Teste",
            ClientPhoneNumber = "(53)99999-0000",
            Address = new Address
            {
                Street = "Rua Exemplo",
                Number = 123,
                Complement = "Sala 5",
                Neighborhood = "Centro",
                City = "Pelotas"
            }
        };

       _clientRepoMock
    .Setup(r => r.GetEntityWithSpecWithoutTrackingAsync(It.IsAny<ISpecification<Client>>()))
    .ReturnsAsync(client);

        _invoiceRepoMock.Setup ( r => r.SumAsync (
                It.IsAny<Expression<Func<BillingInvoice, bool>>> ( ),
                It.IsAny<Expression<Func<BillingInvoice, decimal>>> ( ) ) )
            .ReturnsAsync ( 1100 );

        _paymentRepoMock.Setup ( r => r.SumAsync (
                It.IsAny<Expression<Func<Payment, bool>>> ( ),
                It.IsAny<Expression<Func<Payment, decimal>>> ( ) ) )
            .ReturnsAsync ( 1000 );

        // Act
        var result = await _service.GetClientBasicDashboardAsync(clientId);

        // Assert
        result.ClientId.Should ( ).Be ( clientId );
        result.ClientName.Should ( ).Be ( client.ClientName );
        result.PhoneNumber.Should ( ).Be ( client.ClientPhoneNumber );
        result.TotalInvoiced.Should ( ).Be ( 1100 );
        result.TotalPaid.Should ( ).Be ( 1000 );
        result.Balance.Should ( ).Be ( -100 );
    }


    [Fact]
    public async Task GetClientBasicDashboardAsync_WhenClientNotFound_ShouldThrow ( )
    {
        _clientRepoMock.Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( ( Client? ) null );

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException> ( ( ) =>
            _service.GetClientBasicDashboardAsync ( 999 ) );
    }

}
