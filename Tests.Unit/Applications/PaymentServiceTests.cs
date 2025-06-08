using Applications.Contracts;
using Applications.Dtos.Payments;
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

public class PaymentServiceTests
{
    private readonly Mock<IGenericRepository<Payment>> _paymentRepoMock = new();
    private readonly Mock<IGenericRepository<BillingInvoice>> _invoiceRepoMock = new();
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly IPaymentService _service;

    public PaymentServiceTests ( )
    {
        _uowMock.Setup ( u => u.BeginTransactionAsync ( ) )
            .ReturnsAsync ( Mock.Of<ITransaction> ( ) );

        _service = new PaymentService (
            _paymentRepoMock.Object,
            _invoiceRepoMock.Object,
            _clientRepoMock.Object,
            _uowMock.Object

        );
    }

    [Fact]
    public async Task RegisterClientPaymentAsync_WithValidData_ShouldCreatePaymentAndSetStatusToPartiallyPaid ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId,ClientName = "Test Client" };

        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000,
            PreviousCredit = 0,
            PreviousDebit = 0
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 400,
            Description = "Pagamento parcial",
            PaymentDate = DateTime.Now
        };

        var existingPayments = new List<Payment>
        {
            new() { AmountPaid = 300 }
        };

        _clientRepoMock
    .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
    .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( existingPayments );

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) => p );


        _uowMock
             .Setup ( u => u.SaveChangesAsync ( ) )
             .ReturnsAsync ( 1 );


        // Act
        var result = await _service.RegisterClientPaymentAsync(dto);

        // Assert
        result.Should ( ).NotBeNull ( );
        result.ClientId.Should ( ).Be ( clientId );
        result.AmountPaid.Should ( ).Be ( dto.AmountPaid );
        result.Description.Should ( ).Be ( dto.Description );
        result.BillingInvoiceId.Should ( ).Be ( invoiceId );

        invoice.Status.Should ( ).Be ( InvoiceStatus.PartiallyPaid );

        _paymentRepoMock.Verify ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ), Times.Once );
        _uowMock.Verify ( u => u.SaveChangesAsync ( ), Times.Once );
        _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );
    }



    [Fact]
    public async Task RegisterClientPaymentAsync_WhenTotalPaidEqualsInvoiceAmount_ShouldSetStatusToPaid ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId, ClientName = "Test Client" };

        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000,
            PreviousCredit = 0,
            PreviousDebit = 0
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 600,
            Description = "Quitação",
            PaymentDate = DateTime.Now
        };

        // Simula que o total pago até agora (incluindo o atual) atinge 1000
        var existingPayments = new List<Payment>
    {
        new() { AmountPaid = 400 },
        new() { AmountPaid = 600 } // simula o pagamento atual incluso na soma
    };

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( existingPayments );

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) => p );

        _uowMock
            .Setup ( u => u.SaveChangesAsync ( ) )
            .ReturnsAsync ( 1 );

        // Act
        var result = await _service.RegisterClientPaymentAsync(dto);

        // Assert
        invoice.Status.Should ( ).Be ( InvoiceStatus.Paid );
    }



    [Fact]
    public async Task RegisterClientPaymentAsync_WhenNoOpenInvoice_ShouldThrowUnprocessableEntityException ( )
    {
        // Arrange
        var clientId = 1;
        var client = new Client { ClientId = clientId,ClientName = "Test Client" };

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );
        _invoiceRepoMock
     .Setup ( x => x.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
     .ReturnsAsync ( [] );


        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 200,
            Description = "Sem fatura",
            PaymentDate = DateTime.Now
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnprocessableEntityException> ( ( ) =>
            _service.RegisterClientPaymentAsync ( dto ) );
    }


    [Fact]
    public async Task RegisterClientPaymentAsync_WhenClientNotFound_ShouldThrowNotFoundException ( )
    {
        // Arrange
        _clientRepoMock
            .Setup ( x => x.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( ( Client? ) null );

        var dto = new CreatePaymentDto
        {
            ClientId = 99,
            AmountPaid = 300,
            Description = "Cliente inexistente",
            PaymentDate = DateTime.Now
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException> ( ( ) =>
            _service.RegisterClientPaymentAsync ( dto ) );
    }


    [Fact]
    public async Task RegisterClientPaymentAsync_WhenOverpaid_ShouldStillSetStatusToPaid ( )
    {
        // Arrange
        var clientId = 1;

        var invoice = new BillingInvoice
        {
            BillingInvoiceId = 5,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000,
            PreviousCredit = 0,
            PreviousDebit = 0
        };

        var client = new Client { ClientId = clientId, ClientName = "test name" };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 1200,
            Description = "Pagamento maior que o devido",
            PaymentDate = DateTime.Now
        };

        // Simula que já foi pago 500, e com o atual (1200) totaliza 1700
        var existingPayments = new List<Payment>
    {
        new() { AmountPaid = 500 },
        new() { AmountPaid = 1200 }
    };

        _clientRepoMock
            .Setup ( x => x.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( x => x.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        _paymentRepoMock
            .Setup ( x => x.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( existingPayments );

        _paymentRepoMock
            .Setup ( x => x.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) => p );

        _uowMock
            .Setup ( x => x.SaveChangesAsync ( ) )
            .ReturnsAsync ( 1 );

        // Act
        var result = await _service.RegisterClientPaymentAsync(dto);

        // Assert
        invoice.Status.Should ( ).Be ( InvoiceStatus.Paid );
    }

    [Fact]
    public async Task RegisterClientPaymentAsync_WhenTransactionFails_ShouldRollback ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 500,
            Description = "Test payment",
            PaymentDate = DateTime.Now
        };

        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup ( t => t.RollbackAsync ( ) ).Returns ( Task.CompletedTask );
        transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

        _uowMock
            .Setup ( u => u.BeginTransactionAsync ( ) )
            .ReturnsAsync ( transactionMock.Object );

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) => p );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( new List<Payment> ( ) );

        _uowMock
            .Setup ( u => u.SaveChangesAsync ( ) )
            .ThrowsAsync ( new Exception ( "Simulated failure" ) );

        // Act & Assert
        await Assert.ThrowsAsync<Exception> ( ( ) =>
            _service.RegisterClientPaymentAsync ( dto ) );

        // Verifica se rollback foi chamado
        transactionMock.Verify ( t => t.RollbackAsync ( ), Times.Once );
        _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );
        _paymentRepoMock.Verify ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ), Times.Once );
    }

    [Fact]
    public async Task RegisterClientPaymentAsync_WithNegativeAmount_ShouldThrowException ( )
    {
        // Arrange
        var clientId = 1;
        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = 10,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = -100, // Valor negativo
            Description = "Test payment",
            PaymentDate = DateTime.Now
        };

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        // Act & Assert
        await Assert.ThrowsAsync<UnprocessableEntityException> ( ( ) =>
            _service.RegisterClientPaymentAsync ( dto ) );
    }


    [Fact]
    public async Task RegisterClientPaymentAsync_WithConcurrentPayments_ShouldMaintainConsistency ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000
        };

        var dto1 = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 300,
            Description = "First payment",
            PaymentDate = DateTime.Now
        };

        var dto2 = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 400,
            Description = "Second payment",
            PaymentDate = DateTime.Now
        };

        var paymentsStore = new List<Payment>();
        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
        transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

        _uowMock
            .Setup ( u => u.BeginTransactionAsync ( ) )
            .ReturnsAsync ( transactionMock.Object );

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) =>
            {
                lock ( paymentsStore )
                {
                    paymentsStore.Add ( p );
                }
                return p;
            } );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( ( ) => paymentsStore.ToList ( ) );

        _uowMock
            .Setup ( u => u.SaveChangesAsync ( ) )
            .ReturnsAsync ( 1 );

        // Act
        var task1 = _service.RegisterClientPaymentAsync(dto1);
        var task2 = _service.RegisterClientPaymentAsync(dto2);
        await Task.WhenAll ( task1, task2 );

        // Assert
        paymentsStore.Sum ( p => p.AmountPaid ).Should ( ).Be ( 700 );
        invoice.Status.Should ( ).Be ( InvoiceStatus.PartiallyPaid );
    }


    [Fact]
    public async Task RegisterClientPaymentAsync_WithCreditDebitHistory_ShouldCalculateCorrectly ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;
        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Open,
            TotalServiceOrdersAmount = 1000,
            PreviousCredit = 200,
            PreviousDebit = 100
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 900,
            Description = "Test payment",
            PaymentDate = DateTime.Now
        };

        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
        transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

        _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
        _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        var paymentsList = new List<Payment>(); // controle local da lista

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) =>
            {
                paymentsList.Add ( p ); // simula gravação no "banco"
                return p;
            } );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( ( ) => paymentsList.ToList ( ) );

        // Act
        var result = await _service.RegisterClientPaymentAsync(dto);

        // Assert
        Assert.Equal ( InvoiceStatus.Paid, invoice.Status );
        Assert.Equal ( 900, result.AmountPaid );
    }


    [Theory]
    [InlineData ( InvoiceStatus.Open, 500, InvoiceStatus.PartiallyPaid )]
    [InlineData ( InvoiceStatus.PartiallyPaid, 500, InvoiceStatus.Paid )]
    public async Task RegisterClientPaymentAsync_WithDifferentInvoiceStatus_ShouldUpdateCorrectly (
    InvoiceStatus initialStatus,
    decimal amountPaid,
    InvoiceStatus expectedStatus )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = initialStatus,
            TotalServiceOrdersAmount = 1000,
            PreviousCredit = 0,
            PreviousDebit = 0
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = amountPaid,
            Description = "Test payment",
            PaymentDate = DateTime.Now
        };

        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
        transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

        _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
        _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        // Pagamentos simulados
        var paymentsList = new List<Payment>();
        if ( initialStatus == InvoiceStatus.PartiallyPaid )
        {
            paymentsList.Add ( new Payment { AmountPaid = 500 } );
        }

        _paymentRepoMock
            .Setup ( r => r.CreateAsync ( It.IsAny<Payment> ( ) ) )
            .ReturnsAsync ( ( Payment p ) =>
            {
                paymentsList.Add ( p ); // simula inclusão do pagamento atual
                return p;
            } );

        _paymentRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<Payment>> ( ) ) )
            .ReturnsAsync ( ( ) => paymentsList.ToList ( ) );

        // Act
        var result = await _service.RegisterClientPaymentAsync(dto);

        // Assert
        Assert.Equal ( expectedStatus, invoice.Status );
        Assert.Equal ( amountPaid, result.AmountPaid );
    }

    [Fact]
    public async Task RegisterClientPaymentAsync_WhenInvoiceIsClosed_ShouldThrowUnprocessableEntityException ( )
    {
        // Arrange
        var clientId = 1;
        var invoiceId = 10;

        var client = new Client { ClientId = clientId, ClientName = "Test Client" };
        var invoice = new BillingInvoice
        {
            BillingInvoiceId = invoiceId,
            ClientId = clientId,
            Status = InvoiceStatus.Closed,
            TotalServiceOrdersAmount = 1000
        };

        var dto = new CreatePaymentDto
        {
            ClientId = clientId,
            AmountPaid = 500,
            Description = "Attempt to pay closed invoice",
            PaymentDate = DateTime.Now
        };

        var transactionMock = new Mock<ITransaction>();
        transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

        _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );

        _clientRepoMock
            .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( client );

        _invoiceRepoMock
            .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<BillingInvoice>> ( ) ) )
            .ReturnsAsync ( [invoice] );

        // Act & Assert
        await Assert.ThrowsAsync<UnprocessableEntityException> ( ( ) =>
            _service.RegisterClientPaymentAsync ( dto ) );
    }


}
