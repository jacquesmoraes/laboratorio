using Core.Models.Billing;
using Core.Models.Clients;
using Core.Models.Payments;
using Core.Enums;
using FluentAssertions;
using Moq;
using Applications.Contracts;
using Core.Interfaces;
using Core.Exceptions;
using Core.FactorySpecifications.ClientsFactorySpecifications;
using Applications.Services;
using Core.Models.ServiceOrders;
using Core.Models.Works;

namespace Tests.Unit.Applications;

public class ClientBalanceServiceTests
{
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock;
    private readonly IClientBalanceService _service;

    public ClientBalanceServiceTests ( )
    {
        _clientRepoMock = new Mock<IGenericRepository<Client>> ( );
        _service = new ClientBalanceService ( _clientRepoMock.Object );
    }

    [Fact]
    public async Task GetClientBalanceAsync_WhenClientExists_ReturnsCorrectBalance ( )
    {
        // Arrange
        var clientId = 1;
        // Primeiro, criar o client sem ServiceOrders
        var client = new Client
        {
            ClientId = clientId,
            ClientName = "Test Client",
            Payments = new List<Payment>
            {
        new() { AmountPaid = 500 },
        new() { AmountPaid = 800 }
    }
        };

        // Depois, adicionar os ServiceOrders
        client.ServiceOrders = new List<ServiceOrder>
{
    new()
    {
        DateIn = DateTime.UtcNow,
        Works = new List<Work>(),
        Client = client,
        BillingInvoice = new BillingInvoice
        {
            TotalServiceOrdersAmount = 1000,
            Status = InvoiceStatus.Open,
            PreviousCredit = 100,
            PreviousDebit = 200
        }
    },
    new()
    {
        DateIn = DateTime.UtcNow,
        Works = new List<Work>(),
        Client = client,
        BillingInvoice = new BillingInvoice
        {
            TotalServiceOrdersAmount = 2000,
            Status = InvoiceStatus.Closed,
            PreviousCredit = 0,
            PreviousDebit = 0
        }
    }
};

        _clientRepoMock
    .Setup(repo => repo.GetEntityWithSpec(It.IsAny<ClientSpecification>()))
    .ReturnsAsync(client);



        // Act
        var result = await _service.GetClientBalanceAsync(clientId);

        // Assert
        result.Should ( ).NotBeNull ( );
        result.TotalPaid.Should ( ).Be ( 1300 );
        result.TotalInvoiced.Should ( ).Be ( 3000 );
        result.Debit.Should ( ).Be ( 1700 );
        result.Credit.Should ( ).Be ( 0 );
        result.Balance.Should ( ).Be ( -1700 );
        result.OpenInvoices.Should ( ).HaveCount ( 1 );
        result.ClosedInvoices.Should ( ).HaveCount ( 1 );
    }

    [Fact]
    public async Task GetClientBalanceAsync_WhenClientDoesNotExist_ThrowsNotFoundException ( )
    {
        // Arrange
        var clientId = 1;
        _clientRepoMock.Setup ( repo => repo.GetEntityWithSpec (
    It.IsAny<ClientSpecification> ( ) )
).Returns ( Task.FromResult<Client?> ( null ) );

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException> ( ( ) =>
            _service.GetClientBalanceAsync ( clientId ) );
    }

    [Fact]
    public void FromLists_WithCreditBalance_CalculatesCorrectly ( )
    {
        // Arrange
        var invoices = new List<BillingInvoice>
        {
            new()
            {
                TotalServiceOrdersAmount = 1000,
                Status = InvoiceStatus.Closed,
                PreviousCredit = 200,
                PreviousDebit = 100
            }
        };

        var payments = new List<Payment>
        {
            new() { AmountPaid = 1500 }
        };

        // Act
        var balance = ClientBalance.FromLists(payments, invoices);

        // Assert
        balance.TotalPaid.Should ( ).Be ( 1500 );
        balance.TotalInvoiced.Should ( ).Be ( 1000 );
        balance.Credit.Should ( ).Be ( 500 );
        balance.Debit.Should ( ).Be ( 0 );
        balance.Balance.Should ( ).Be ( 500 );
    }

    [Fact]
    public void FromLists_WithMultipleInvoicesAndPayments_CalculatesCorrectly ( )
    {
        // Arrange
        var invoices = new List<BillingInvoice>
        {
            new()
            {
                TotalServiceOrdersAmount = 1000,
                Status = InvoiceStatus.Open,
                PreviousCredit = 0,
                PreviousDebit = 0
            },
            new()
            {
                TotalServiceOrdersAmount = 2000,
                Status = InvoiceStatus.PartiallyPaid,
                PreviousCredit = 100,
                PreviousDebit = 200
            },
            new()
            {
                TotalServiceOrdersAmount = 3000,
                Status = InvoiceStatus.Closed,
                PreviousCredit = 0,
                PreviousDebit = 0
            }
        };

        var payments = new List<Payment>
        {
            new() { AmountPaid = 1000 },
            new() { AmountPaid = 2000 }
        };

        // Act
        var balance = ClientBalance.FromLists(payments, invoices);

        // Assert
        balance.TotalPaid.Should ( ).Be ( 3000 );
        balance.TotalInvoiced.Should ( ).Be ( 6000 );
        balance.Debit.Should ( ).Be ( 3000 );
        balance.Credit.Should ( ).Be ( 0 );
        balance.Balance.Should ( ).Be ( -3000 );
        balance.OpenInvoices.Should ( ).HaveCount ( 2 ); // Inclui Open e PartiallyPaid
        balance.ClosedInvoices.Should ( ).HaveCount ( 1 );
    }

    [Fact]
    public void FromLists_WithNoInvoicesAndPayments_ReturnsZeroBalance ( )
    {
        // Arrange
        var invoices = new List<BillingInvoice>();
        var payments = new List<Payment>();

        // Act
        var balance = ClientBalance.FromLists(payments, invoices);

        // Assert
        balance.TotalPaid.Should ( ).Be ( 0 );
        balance.TotalInvoiced.Should ( ).Be ( 0 );
        balance.Credit.Should ( ).Be ( 0 );
        balance.Debit.Should ( ).Be ( 0 );
        balance.Balance.Should ( ).Be ( 0 );
        balance.OpenInvoices.Should ( ).BeEmpty ( );
        balance.ClosedInvoices.Should ( ).BeEmpty ( );
    }

    [Fact]
    public void CurrentOpenInvoice_ReturnsMostRecentOpenInvoice ( )
    {
        // Arrange
        var invoices = new List<BillingInvoice>
        {
            new()
            {
                TotalServiceOrdersAmount = 1000,
                Status = InvoiceStatus.Open,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                TotalServiceOrdersAmount = 2000,
                Status = InvoiceStatus.Open,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                TotalServiceOrdersAmount = 3000,
                Status = InvoiceStatus.Closed,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var payments = new List<Payment>();

        // Act
        var balance = ClientBalance.FromLists(payments, invoices);

        // Assert
        balance.CurrentOpenInvoice.Should ( ).NotBeNull ( );
        balance.CurrentOpenInvoice!.TotalServiceOrdersAmount.Should ( ).Be ( 2000 );
    }

    [Fact]
    public void FromLists_WithPreviousCreditAndDebit_CalculatesCorrectly ( )
    {
        // Arrange
        var invoices = new List<BillingInvoice>
        {
            new()
            {
                TotalServiceOrdersAmount = 1000,
                Status = InvoiceStatus.Open,
                PreviousCredit = 500,
                PreviousDebit = 200
            }
        };

        var payments = new List<Payment>
        {
            new() { AmountPaid = 800 }
        };

        // Act
        var balance = ClientBalance.FromLists(payments, invoices);

        // Assert
        balance.TotalPaid.Should ( ).Be ( 800 );
        balance.TotalInvoiced.Should ( ).Be ( 1000 );
        balance.Debit.Should ( ).Be ( 200 );
        balance.Credit.Should ( ).Be ( 0 );
        balance.Balance.Should ( ).Be ( -200 );
    }
    [Fact]
public void FromLists_WithAllPartiallyPaidInvoices_CalculatesCorrectly()
{
    // Arrange
    var invoices = new List<BillingInvoice>
    {
        new()
        {
            TotalServiceOrdersAmount = 1000,
            Status = InvoiceStatus.PartiallyPaid,
            PreviousCredit = 200,
            PreviousDebit = 100
        },
        new()
        {
            TotalServiceOrdersAmount = 2000,
            Status = InvoiceStatus.PartiallyPaid,
            PreviousCredit = 300,
            PreviousDebit = 150
        }
    };

    var payments = new List<Payment>
    {
        new() { AmountPaid = 500 },
        new() { AmountPaid = 1000 }
    };

    // Act
    var balance = ClientBalance.FromLists(payments, invoices);

    // Assert
    balance.TotalPaid.Should().Be(1500);
    balance.TotalInvoiced.Should().Be(3000);
    balance.Debit.Should().Be(1500);
    balance.Credit.Should().Be(0);
    balance.Balance.Should().Be(-1500);
    balance.OpenInvoices.Should().HaveCount(2);
    balance.ClosedInvoices.Should().HaveCount(0);
}

[Fact]
public void FromLists_WithVariousPreviousCreditAndDebit_CalculatesCorrectly()
{
    // Arrange
    var invoices = new List<BillingInvoice>
    {
        new()
        {
            TotalServiceOrdersAmount = 1000,
            Status = InvoiceStatus.Open,
            PreviousCredit = 500,  // Crédito maior que o débito
            PreviousDebit = 200
        },
        new()
        {
            TotalServiceOrdersAmount = 2000,
            Status = InvoiceStatus.Open,
            PreviousCredit = 100,  // Crédito menor que o débito
            PreviousDebit = 300
        },
        new()
        {
            TotalServiceOrdersAmount = 3000,
            Status = InvoiceStatus.Closed,
            PreviousCredit = 0,    // Sem crédito nem débito
            PreviousDebit = 0
        }
    };

    var payments = new List<Payment>
    {
        new() { AmountPaid = 1500 },
        new() { AmountPaid = 2500 }
    };

    // Act
    var balance = ClientBalance.FromLists(payments, invoices);

    // Assert
    balance.TotalPaid.Should().Be(4000);
    balance.TotalInvoiced.Should().Be(6000);
    balance.Debit.Should().Be(2000);
    balance.Credit.Should().Be(0);
    balance.Balance.Should().Be(-2000);
    balance.OpenInvoices.Should().HaveCount(2);
    balance.ClosedInvoices.Should().HaveCount(1);
}
}