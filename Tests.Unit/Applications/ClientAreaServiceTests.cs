using Applications.Services;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Specifications;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Applications;

public class ClientAreaServiceTests
{
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock = new();
    private readonly Mock<IGenericRepository<BillingInvoice>> _invoiceRepoMock = new();
    private readonly Mock<IGenericRepository<Payment>> _paymentRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();

    private readonly ClientAreaService _service;

    public ClientAreaServiceTests ( )
    {
        _uowMock.Setup ( x => x.Repository<Client> ( ) ).Returns ( _clientRepoMock.Object );
        _uowMock.Setup ( x => x.Repository<BillingInvoice> ( ) ).Returns ( _invoiceRepoMock.Object );
        _uowMock.Setup ( x => x.Repository<Payment> ( ) ).Returns ( _paymentRepoMock.Object );

        _service = new ClientAreaService (
            _clientRepoMock.Object,
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
            _uowMock.Object
        );
    }

    [Fact]
    public async Task GetClientDashboardAsync_ShouldReturnCorrectData ( )
    {
        // Arrange
        var client = new Client
        {
            ClientId = 1,
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

        var invoices = new List<BillingInvoice>
        {
            new()
            {
                BillingInvoiceId = 1,
                InvoiceNumber = "FAT-001",
                CreatedAt = DateTime.UtcNow,
                Description = "Fatura de Teste",
                Client = client,
                ClientId = client.ClientId,
                TotalServiceOrdersAmount = 1000,
                
                PreviousCredit = 0,
                PreviousDebit = 100,
                Status = InvoiceStatus.Closed,
                Payments = new List<Payment>
                {
                    new() { AmountPaid = 500 },
                    new() { AmountPaid = 200 }
                },
                ServiceOrders = []
            }
        };

        var payments = new List<Payment>
        {
            new() { AmountPaid = 700 },
            new() { AmountPaid = 300 }
        };

        _clientRepoMock.Setup ( repo =>
                repo.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock.Setup ( repo =>
                repo.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( invoices );

        _paymentRepoMock.Setup ( repo =>
                repo.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( payments );

        // Act
        var result = await _service.GetClientDashboardAsync(client.ClientId);

        // Assert
        result.Should ( ).NotBeNull ( );
        result.Client.Should ( ).Be ( client );
        result.TotalPaid.Should ( ).Be ( 1000 );
        result.TotalInvoiced.Should ( ).Be ( 1100 );
        result.Credit.Should ( ).Be ( 0 );
        result.Debit.Should ( ).Be ( 100 );
        result.Invoices.Should ( ).HaveCount ( 1 );
        result.Invoices.First ( ).InvoiceNumber.Should ( ).Be ( "FAT-001" );
    }

    [Fact]
    public async Task GetClientDashboardAsync_WhenClientNotFound_ShouldThrow ( )
    {
        // Arrange
        _clientRepoMock
            .Setup ( repo => repo.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( ( Client? ) null );

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException> ( ( ) =>
            _service.GetClientDashboardAsync ( 999 ) );
    }
}
