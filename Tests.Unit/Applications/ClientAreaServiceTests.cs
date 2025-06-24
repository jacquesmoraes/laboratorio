using Applications.Services;
using Applications.Records.Payments;
using Applications.Projections.ServiceOrder;
using Applications.Responses;
using AutoMapper;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Models.ServiceOrders;
using Core.Params;
using Core.Specifications;
using FluentAssertions;
using Moq;
using Core.Models.Works;
using System.Linq.Expressions;

namespace Tests.Unit.Applications;

public class ClientAreaServiceTests
{
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock = new();
    private readonly Mock<IGenericRepository<BillingInvoice>> _invoiceRepoMock = new();
    private readonly Mock<IGenericRepository<Payment>> _paymentRepoMock = new();
    private readonly Mock<IGenericRepository<ServiceOrder>> _orderRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly IMapper _mapper;

    private readonly ClientAreaService _service;

    public ClientAreaServiceTests ( )
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ServiceOrder, ServiceOrderListProjection>();
            cfg.CreateMap<Payment, ClientPaymentRecord>();
        });

        _mapper = mapperConfig.CreateMapper ( );

        _service = new ClientAreaService (
            _clientRepoMock.Object,
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
            _orderRepoMock.Object,
            _uowMock.Object,
            _mapper );
    }

   [Fact]
public async Task GetClientBasicDashboardAsync_ShouldReturnCorrectBalanceInfo()
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

    _clientRepoMock.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Client>>()))
        .ReturnsAsync(client);

    _invoiceRepoMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<BillingInvoice, bool>>>(),
            It.IsAny<Expression<Func<BillingInvoice, decimal>>>()))
        .ReturnsAsync(1100);

    _paymentRepoMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<Payment, bool>>>(),
            It.IsAny<Expression<Func<Payment, decimal>>>()))
        .ReturnsAsync(1000);

    // Act
    var result = await _service.GetClientBasicDashboardAsync(clientId);

    // Assert
    result.ClientId.Should().Be(clientId);
    result.ClientName.Should().Be(client.ClientName);
    result.PhoneNumber.Should().Be(client.ClientPhoneNumber);
    result.TotalInvoiced.Should().Be(1100);
    result.TotalPaid.Should().Be(1000);
    result.Balance.Should().Be(-100);
}


    [Fact]
public async Task GetClientBasicDashboardAsync_WhenClientNotFound_ShouldThrow()
{
    _clientRepoMock.Setup(r => r.GetEntityWithSpec(It.IsAny<ISpecification<Client>>()))
        .ReturnsAsync((Client?)null);

    // Act + Assert
    await Assert.ThrowsAsync<NotFoundException>(() =>
        _service.GetClientBasicDashboardAsync(999));
}

}
