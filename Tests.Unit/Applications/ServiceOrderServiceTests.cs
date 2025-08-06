using Applications.Contracts;
using Applications.Dtos.ServiceOrder;
using Applications.Projections.ServiceOrder;
using Applications.Services;
using AutoMapper;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.Schedule;
using Core.Models.ServiceOrders;
using Core.Models.Works;
using Core.Params;
using Core.Specifications;
using FluentAssertions;
using Moq;

namespace Tests.Unit.Applications
{
    public class ServiceOrderServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IGenericRepository<ServiceOrder>> _orderRepoMock;
        private readonly Mock<IGenericRepository<ServiceOrderSchedule>> _scheduleRepoMock;
        private readonly Mock<IGenericRepository<Client>> _clientRepoMock;
        private readonly Mock<IGenericRepository<Sector>> _sectorRepoMock;
        private readonly Mock<IPerformanceLoggingService> _perfLoggerMock;
        private readonly Mock<IUnitOfWork> _uowMock;

        private readonly IServiceOrderService _service;

        public ServiceOrderServiceTests ( )
        {
            _mapperMock = new Mock<IMapper> ( );
            _orderRepoMock = new Mock<IGenericRepository<ServiceOrder>> ( );
            _clientRepoMock = new Mock<IGenericRepository<Client>> ( );
            _sectorRepoMock = new Mock<IGenericRepository<Sector>> ( );
            _uowMock = new Mock<IUnitOfWork> ( );
            _scheduleRepoMock = new Mock<IGenericRepository<ServiceOrderSchedule>> ( );
            _perfLoggerMock = new Mock<IPerformanceLoggingService> ( );

            _service = new ServiceOrderService (
                _mapperMock.Object,
                _orderRepoMock.Object,
                _clientRepoMock.Object,
                _sectorRepoMock.Object,
                _scheduleRepoMock.Object,
                _uowMock.Object,
                _perfLoggerMock.Object
                );
        }

        [Fact]
        public async Task CreateOrderAsync_WithValidData_ShouldCreateOrder ( )
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client" };
            var sector = new Sector { SectorId = 1, Name = "Test Sector" };

            var dto = new CreateServiceOrderDto
            {
                ClientId = 1,
                FirstSectorId = 1,
                DateIn = DateTime.UtcNow,
                PatientName = "Test Patient",
                Works = new List<CreateWorkDto>
        {
            new() { WorkTypeId = 1, PriceUnit = 100, Quantity = 2 } // Total: 200
        }
            };

            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Client = client,
                ClientId = client.ClientId,
                PatientName = dto.PatientName,
                Status = OrderStatus.Production,
                DateIn = dto.DateIn,
                Works = new List<Work>
            {
                new() { Quantity = 2, PriceUnit = 100 } // PriceTotal será 200
            },

                Stages = new List<ProductionStage>()
            };

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
            transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

            _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
            _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

            _clientRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
                .ReturnsAsync ( client );

            _sectorRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Sector>> ( ) ) )
                .ReturnsAsync ( sector );

            _mapperMock
                .Setup ( m => m.Map<ServiceOrder> ( dto ) )
                .Returns ( order );

            _orderRepoMock
                .Setup ( r => r.CreateAsync ( It.IsAny<ServiceOrder> ( ) ) )
                .ReturnsAsync ( ( ServiceOrder o ) => o );
            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );
            _orderRepoMock
                .Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( [] ); // ou um List<ServiceOrder> com 1 elemento fictício, se quiser simular uma ordem anterior

            // Act
            var result = await _service.CreateOrderAsync(dto);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.ClientId.Should ( ).Be ( client.ClientId );
            result.Status.Should ( ).Be ( OrderStatus.Production );
            result.Stages.Should ( ).HaveCount ( 1 );
            result.OrderTotal.Should ( ).Be ( 200 );

            _orderRepoMock.Verify ( r => r.CreateAsync ( It.IsAny<ServiceOrder> ( ) ), Times.Once );

            _uowMock.Verify ( u => u.SaveChangesAsync ( ), Times.Once );
            _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );

        }

        [Fact]
        public async Task MoveToStageAsync_WithValidData_ShouldMoveOrder ( )
        {
            // Arrange
            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
            transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

            _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
            _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

            var dto = new MoveToStageDto
            {
                ServiceOrderId = 1,
                SectorId = 2,
                DateIn = DateTime.UtcNow
            };

            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Production,
                DateIn = dto.DateIn,
                Works = new List<Work>(),
                Client = new Client { ClientId = 1, ClientName = "Test Client" },
                Stages = new List<ProductionStage>()
            };

            var sector = new Sector { SectorId = 2, Name = "New Sector" };

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            _sectorRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Sector>> ( ) ) )
                .ReturnsAsync ( sector );

            // Act
            var result = await _service.MoveToStageAsync(dto);

            // Assert
            result.Should ( ).NotBeNull ( );
            result!.Stages.Should ( ).HaveCount ( 1 );
            result.Stages [0].SectorId.Should ( ).Be ( sector.SectorId );

            // Verifica interações com mocks
            _orderRepoMock.Verify ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ), Times.Once );
            _sectorRepoMock.Verify ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Sector>> ( ) ), Times.Once );
            _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );
            _uowMock.Verify ( u => u.SaveChangesAsync ( ), Times.Once );
        }


        [Fact]
        public async Task FinishOrdersAsync_WithValidData_ShouldFinishOrders ( )
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "Test Client" };
            var sector = new Sector { SectorId = 1, Name = "Setor A" };

            var order1 = new ServiceOrder
            {
                ServiceOrderId = 1,
                ClientId = client.ClientId,
                Client = client,
                Status = OrderStatus.Production,
                Works = [],
                DateIn = DateTime.UtcNow.AddDays(-5),
                Stages = []
            };
            var order2 = new ServiceOrder
            {
                ServiceOrderId = 2,
                ClientId = client.ClientId,
                Client = client,
                Status = OrderStatus.Production,
                Works = [],
                DateIn = DateTime.UtcNow.AddDays(-5),
                Stages = []
            };

            // Atribui o ServiceOrder nas stages após criação
            order1.Stages.Add ( new ProductionStage
            {
                DateIn = DateTime.UtcNow.AddDays ( -3 ),
                DateOut = null,
                SectorId = sector.SectorId,
                Sector = sector,
                ServiceOrder = order1
            } );
            order2.Stages.Add ( new ProductionStage
            {
                DateIn = DateTime.UtcNow.AddDays ( -3 ),
                DateOut = null,
                SectorId = sector.SectorId,
                Sector = sector,
                ServiceOrder = order2
            } );

            var dto = new FinishOrderDto
            {
                ServiceOrderIds = new List<int> { 1, 2 },
                DateOut = DateTime.UtcNow
            };

            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
            transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

            _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
            _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

            // Mock para GetEntityWithSpec com verificação direta do ID
            var queue = new Queue<ServiceOrder>([order1, order2]);
            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( ( ISpecification<ServiceOrder> _ ) =>
                {
                    return queue.Dequeue ( );
                } );

            // Act
            var result = await _service.FinishOrdersAsync(dto);

            // Assert
            result.Should ( ).HaveCount ( 2 );
            result.All ( o => o.Status == OrderStatus.Finished ).Should ( ).BeTrue ( );
            result.All ( o => o.DateOutFinal.HasValue ).Should ( ).BeTrue ( );

            _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );
            _uowMock.Verify ( u => u.SaveChangesAsync ( ), Times.Once );
            _orderRepoMock.Verify ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ), Times.Exactly ( 2 ) );
        }

        [Fact]
        public async Task UpdateOrderAsync_WithValidData_ShouldUpdateOrder ( )
        {
            // Arrange
            var dto = new CreateServiceOrderDto
            {
                ClientId = 1,
                PatientName = "Updated Patient",
                Works = new List<CreateWorkDto>()
            };
            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                DateIn = dto.DateIn,
                Status = OrderStatus.Production,
                ClientId = dto.ClientId,
                Client = new Client { ClientId = dto.ClientId, ClientName = "Original Client" },
                Works = new List<Work>()
            };



            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            _mapperMock
                .Setup ( m => m.Map<List<Work>> ( dto.Works ) )
                .Returns ( new List<Work> ( ) );

            // Act
            var result = await _service.UpdateOrderAsync(1, dto);

            // Assert
            result.Should ( ).NotBeNull ( );
            result!.PatientName.Should ( ).Be ( dto.PatientName );
        }

        [Fact]
        public async Task DeleteOrderAsync_WithValidData_ShouldDeleteOrder ( )
        {
            // Arrange
            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Production,
                ClientId = 1,
                Client = new Client { ClientId = 1, ClientName = "Original Client" },
                DateIn = DateTime.UtcNow,
                Works = new List<Work>()
            };


            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act
            var result = await _service.DeleteOrderAsync(1);

            // Assert
            result.Should ( ).NotBeNull ( );
            _orderRepoMock.Verify ( r => r.DeleteAsync ( 1 ), Times.Once );
        }

        [Fact]
        public async Task ReopenOrderAsync_WithValidData_ShouldReopenOrder ( )
        {
            // Arrange
            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Finished,
                DateOutFinal = DateTime.UtcNow,
                ClientId = 1,
                Client = new Client { ClientId = 1, ClientName = "Original Client" },
                DateIn = DateTime.UtcNow,
                Works = new List<Work>(),
                Stages = []
            };

            var sector = new Sector { SectorId = 1, Name = "Setor A" };

            var stage = new ProductionStage
            {
                SectorId = sector.SectorId,
                Sector = sector,
                DateIn = DateTime.UtcNow.AddDays(-2),
                DateOut = DateTime.UtcNow,
                ServiceOrder = order // importante!
            };

            order.Stages.Add ( stage );

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act
            var result = await _service.ReopenOrderAsync(1);

            // Assert
            result.Should ( ).NotBeNull ( );
            result!.Status.Should ( ).Be ( OrderStatus.Production );
            result.DateOutFinal.Should ( ).BeNull ( );
            result.Stages [0].DateOut.Should ( ).BeNull ( );
        }


        [Fact]
        public async Task CreateOrderAsync_WithNonExistingClient_ShouldThrowNotFoundException ( )
        {
            // Arrange
            var dto = new CreateServiceOrderDto
            {
                ClientId = 999,
                FirstSectorId = 1,
                DateIn = DateTime.UtcNow,
                Works = []
            };

            _clientRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
                .ReturnsAsync ( ( Client? ) null );

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException> ( ( ) => _service.CreateOrderAsync ( dto ) );
        }


        [Fact]
        public async Task FinishOrdersAsync_WithDifferentClients_ShouldThrowBusinessRuleException ( )
        {
            // Arrange
            var client1 = new Client { ClientId = 1, ClientName = "Client 1" };
            var client2 = new Client { ClientId = 2, ClientName = "Client 2" };
            var sector = new Sector { SectorId = 1, Name = "Setor A" };

            var order1 = new ServiceOrder
            {
                ServiceOrderId = 1,
                ClientId = 1,
                Client = client1,
                Status = OrderStatus.Production,
                Works = [],
                DateIn = DateTime.UtcNow,
                Stages = [
            new ProductionStage
            {
                SectorId = sector.SectorId,
                Sector = sector,
                DateIn = DateTime.UtcNow.AddDays(-2),
                ServiceOrder = null!
            }
        ]
            };

            var order2 = new ServiceOrder
            {
                ServiceOrderId = 2,
                ClientId = 2,
                Client = client2,
                Status = OrderStatus.Production,
                Works = [],
                DateIn = DateTime.UtcNow,
                Stages = [
            new ProductionStage
            {
                SectorId = sector.SectorId,
                Sector = sector,
                DateIn = DateTime.UtcNow.AddDays(-2),
                ServiceOrder = null!
            }
        ]
            };

            var dto = new FinishOrderDto
            {
                ServiceOrderIds = new List<int> { 1, 2 },
                DateOut = DateTime.UtcNow
            };

            var queue = new Queue<ServiceOrder>([order1, order2]);
            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( ( ) => queue.Dequeue ( ) );

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException> ( ( ) => _service.FinishOrdersAsync ( dto ) );
        }


        [Fact]
        public async Task UpdateOrderAsync_WhenOrderIsFinished_ShouldThrowBusinessRuleException ( )
        {
            // Arrange
            var dto = new CreateServiceOrderDto
            {
                ClientId = 1,
                PatientName = "Paciente",
                Works = []
            };

            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Finished,
                Client = new Client { ClientId = 1, ClientName= "client test" },
                Works = [],
                DateIn = DateTime.UtcNow
            };

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException> ( ( ) => _service.UpdateOrderAsync ( 1, dto ) );
        }


        [Fact]
        public async Task SendToTryInAsync_WithInvalidDate_ShouldThrowValidationException ( )
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "client test" };
            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Production,
                DateIn = DateTime.UtcNow,
                Works = [],
                ClientId = client.ClientId,
                Client = client,
                Stages = []
            };

            var sector = new Sector { SectorId = 1, Name = "Setor Teste" };

            var stage = new ProductionStage
            {
                DateIn = DateTime.UtcNow.AddDays(-1),
                DateOut = null,
                SectorId = sector.SectorId,
                Sector = sector,
                ServiceOrder = order
            };

            order.Stages.Add ( stage );

            var dto = new SendToTryInDto
            {
                ServiceOrderId = 1,
                DateOut = DateTime.UtcNow.AddDays(-2) // inválida: anterior à última entrada
            };

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act & Assert
            await Assert.ThrowsAsync<UnprocessableEntityException> ( ( ) => _service.SendToTryInAsync ( dto ) );
        }



        [Fact]
        public async Task SendToTryInAsync_WithValidData_ShouldSendToTryIn ( )
        {
            // Arrange
            var sector = new Sector { SectorId = 1, Name = "Setor A" };
            var client = new Client { ClientId = 1, ClientName = "client test" };

            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Production,
                DateIn = DateTime.UtcNow,
                Works = [],
                Client = client,
                ClientId = client.ClientId,
                Stages = []
            };

            var stage = new ProductionStage
            {
                DateIn = DateTime.UtcNow.AddDays(-1),
                SectorId = sector.SectorId,
                Sector = sector,
                ServiceOrder = order
            };

            order.Stages.Add ( stage );

            var localNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
           var dto = new SendToTryInDto
{
    ServiceOrderId = 1,
    DateOut = localNow
};

            // Configurar o mock da transação
            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup ( t => t.CommitAsync ( ) ).Returns ( Task.CompletedTask );
            transactionMock.Setup ( t => t.DisposeAsync ( ) ).Returns ( ValueTask.CompletedTask );

            _uowMock.Setup ( u => u.BeginTransactionAsync ( ) ).ReturnsAsync ( transactionMock.Object );
            _uowMock.Setup ( u => u.SaveChangesAsync ( ) ).ReturnsAsync ( 1 );

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act
            var result = await _service.SendToTryInAsync(dto);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Status.Should ( ).Be ( OrderStatus.TryIn );
            result.Stages.Last().DateOut.Should().BeCloseTo(dto.DateOut.ToUniversalTime(), TimeSpan.FromSeconds(1));


            // Verificar interações com mocks
            _uowMock.Verify ( u => u.BeginTransactionAsync ( ), Times.Once );
            _uowMock.Verify ( u => u.SaveChangesAsync ( ), Times.Once );
        }

        [Fact]
        public async Task SendToTryInAsync_WithNoOpenStage_ShouldThrowException ( )
        {
            // Arrange
            var client = new Client { ClientId = 1, ClientName = "client test" };
            var sector = new Sector { SectorId = 1, Name = "Setor A" };

            var order = new ServiceOrder
            {
                ServiceOrderId = 1,
                Status = OrderStatus.Production,
                DateIn = DateTime.UtcNow,
                ClientId = client.ClientId,
                Client = client,
                Works = new List<Work>(),
                Stages = []
            };

            var closedStage = new ProductionStage
            {
                DateIn = DateTime.UtcNow.AddDays(-1),
                DateOut = DateTime.UtcNow, // <- já fechado
                SectorId = sector.SectorId,
                Sector = sector,
                ServiceOrder = order
            };

            order.Stages.Add ( closedStage );

            var dto = new SendToTryInDto
            {
                ServiceOrderId = 1,
                DateOut = DateTime.UtcNow
            };

            _orderRepoMock
                .Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( order );

            // Act & Assert
            await Assert.ThrowsAsync<UnprocessableEntityException> ( ( ) => _service.SendToTryInAsync ( dto ) );

        }

        [Fact]
        public async Task GetPaginatedAsync_WithClientFilter_ShouldReturnOnlyClientOrders ( )
        {
            // Arrange
            var clientId = 1;
            var param = new ServiceOrderParams
            {
                ClientId = clientId,
                PageNumber = 1,
                PageSize = 10
            };
            var client = new Client { ClientId = clientId, ClientName = "Cliente Teste" };

            var orders = new List<ServiceOrder>
            {
                new() { ServiceOrderId = 1, ClientId = clientId,Client = client , PatientName = "A", DateIn = DateTime.UtcNow,  Works = new List<Work>(), Stages = new List<ProductionStage>()  },
                new() { ServiceOrderId = 2, ClientId = clientId,Client = client , PatientName = "A", DateIn = DateTime.UtcNow,  Works = new List<Work>(), Stages = new List<ProductionStage>()  }
            };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.Count );
            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( orders ) )
                .Returns ( orders.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    PatientName = o.PatientName,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).HaveCount ( 2 );
            result.Data.All ( x => x.ServiceOrderId == 1 || x.ServiceOrderId == 2 ).Should ( ).BeTrue ( );
        }


        [Fact]
        public async Task GetPaginatedAsync_WithStatusFilter_ShouldReturnOnlyMatchingStatus ( )
        {
            // Arrange
            var param = new ServiceOrderParams
            {
                Status = OrderStatus.TryIn,
                PageNumber = 1,
                PageSize = 10
            };

            var client = new Client { ClientId = 1, ClientName = "Cliente Teste" };

            var orders = new List<ServiceOrder>
    {
        new() { ServiceOrderId = 1, ClientId = 1, Client = client, PatientName = "Paciente 1", DateIn = DateTime.UtcNow, Status = OrderStatus.TryIn, Works = [], Stages = [] },
        new() { ServiceOrderId = 2, ClientId = 1, Client = client, PatientName = "Paciente 2", DateIn = DateTime.UtcNow, Status = OrderStatus.TryIn, Works = [], Stages = [] }
    };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.Count );
            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( orders ) )
                .Returns ( orders.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    PatientName = o.PatientName,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).OnlyContain ( x => x.Status == OrderStatus.TryIn.ToString ( ) );
        }

        [Fact]
        public async Task GetPaginatedAsync_WithPatientNameFilter_ShouldReturnMatchingOrders ( )
        {
            // Arrange
            var param = new ServiceOrderParams
            {
                PatientName = "jo", // deve bater com "João", "JOSE", "Marjory", etc.
                PageNumber = 1,
                PageSize = 10
            };

            var client = new Client { ClientId = 1, ClientName = "Cliente Teste" };

            var orders = new List<ServiceOrder>
            {
                new() { ServiceOrderId = 1, ClientId = 1, Client = client, PatientName = "João da Silva", DateIn = DateTime.UtcNow, Works = [], Stages = [] },
                new() { ServiceOrderId = 2, ClientId = 1, Client = client, PatientName = "Marjory Alves", DateIn = DateTime.UtcNow, Works = [], Stages = [] }
            };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.Count );
            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( orders ) )
                .Returns ( orders.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    PatientName = o.PatientName,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).OnlyContain ( x =>
                x.PatientName.Contains ( "jo", StringComparison.OrdinalIgnoreCase ) );
        }


        [Fact]
        public async Task GetPaginatedAsync_WithDateRange_ShouldReturnOrdersWithinRange ( )
        {
            // Arrange
            var start = DateTime.UtcNow.AddDays(-5);
            var end = DateTime.UtcNow.AddDays(-1);

            var param = new ServiceOrderParams
            {
                StartDate = start,
                EndDate = end,
                PageNumber = 1,
                PageSize = 10
            };

            var client = new Client { ClientId = 1, ClientName = "Cliente Teste" };

            var orders = new List<ServiceOrder>
    {
        new() { ServiceOrderId = 1, ClientId = 1, Client = client, DateIn = DateTime.UtcNow.AddDays(-4), Works = [], Stages = [] },
        new() { ServiceOrderId = 2, ClientId = 1, Client = client, DateIn = DateTime.UtcNow.AddDays(-3), Works = [], Stages = [] }
    };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.Count );
            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( orders ) )
                .Returns ( orders.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).OnlyContain ( o => o.DateIn >= start && o.DateIn <= end );
        }


        [Fact]
        public async Task GetPaginatedAsync_WithPageSize_ShouldReturnLimitedResults ( )
        {
            // Arrange
            var param = new ServiceOrderParams
            {
                PageNumber = 1,
                PageSize = 2
            };

            var client = new Client { ClientId = 1, ClientName = "Cliente Teste" };

            var allOrders = new List<ServiceOrder>
    {
        new() { ServiceOrderId = 1, ClientId = 1, Client = client, DateIn = DateTime.UtcNow, Works = [], Stages = [] },
        new() { ServiceOrderId = 2, ClientId = 1, Client = client, DateIn = DateTime.UtcNow, Works = [], Stages = [] },
        new() { ServiceOrderId = 3, ClientId = 1, Client = client, DateIn = DateTime.UtcNow, Works = [], Stages = [] }
    };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( allOrders.Count );

            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( allOrders.Take ( 2 ).ToList ( ) );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( It.IsAny<List<ServiceOrder>> ( ) ) )
                .Returns ( ( List<ServiceOrder> input ) => input.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).HaveCount ( 2 );
            result.TotalItems.Should ( ).Be ( 3 );
        }

        [Fact]
        public async Task GetPaginatedAsync_WithSortByDateIn_ShouldReturnOrderedResults ( )
        {
            // Arrange
            var param = new ServiceOrderParams
            {
                Sort = "DateInDesc",
                PageNumber = 1,
                PageSize = 10
            };

            var client = new Client { ClientId = 1, ClientName = "Cliente Teste" };

            var orders = new List<ServiceOrder>
    {
        new() { ServiceOrderId = 1, ClientId = 1, Client = client, DateIn = DateTime.UtcNow.AddDays(-1), Works = [], Stages = [] },
        new() { ServiceOrderId = 2, ClientId = 1, Client = client, DateIn = DateTime.UtcNow.AddDays(-2), Works = [], Stages = [] }
    };

            _orderRepoMock.Setup ( r => r.CountAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.Count );
            _orderRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<ServiceOrder>> ( ) ) )
                .ReturnsAsync ( orders.OrderByDescending ( o => o.DateIn ).ToList ( ) );

            _mapperMock.Setup ( m => m.Map<IReadOnlyList<ServiceOrderListProjection>> ( It.IsAny<List<ServiceOrder>> ( ) ) )
                .Returns ( ( List<ServiceOrder> input ) => input.Select ( o => new ServiceOrderListProjection
                {
                    ServiceOrderId = o.ServiceOrderId,
                    DateIn = o.DateIn,
                    Status = o.Status.ToString ( )
                } ).ToList ( ) );

            // Act
            var result = await _service.GetPaginatedAsync(param);

            // Assert
            result.Should ( ).NotBeNull ( );
            result.Data.Should ( ).BeInDescendingOrder ( o => o.DateIn );
        }



    }
}