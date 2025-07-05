using Applications.Dtos.Pricing;
using Applications.Services;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.Pricing;
using Core.Models.Works;
using Core.Specifications;
using Moq;

namespace Tests.Unit.Applications;

public class TablePriceServiceTests
{
    private readonly Mock<IGenericRepository<TablePrice>> _tableRepoMock = new();
    private readonly Mock<IGenericRepository<Client>> _clientRepoMock = new();
    private readonly Mock<IGenericRepository<WorkType>> _workTypeRepoMock = new();

    private readonly TablePriceService _service;

    public TablePriceServiceTests ( )
    {
        _service = new TablePriceService (
            _tableRepoMock.Object,
            _clientRepoMock.Object,
            _workTypeRepoMock.Object
        );
    }

    [Fact]
    public async Task Create_ValidTablePriceWithItems_ShouldSucceed ( )
    {
        var dto = new CreateTablePriceDto
        {
            Name = "Tabela verão",
            Description = "Descontos especiais",
            Items = new List<TablePriceItemInputDto>
            {
                new() { WorkTypeId = 1, Price = 250 },
                new() { WorkTypeId = 2, Price = 100 }
            }
        };

        var fakeWorkTypes = new List<WorkType>
        {
            new() { Id = 1, Name = "PSI", IsActive = true, WorkSectionId = 1 },
            new() { Id = 2, Name = "Provisório", IsActive = true, WorkSectionId = 1 }
        };

        _workTypeRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<WorkType>> ( ) ) )
            .ReturnsAsync ( fakeWorkTypes );

        _tableRepoMock.Setup ( r => r.CreateAsync ( It.IsAny<TablePrice> ( ) ) )
            .ReturnsAsync ( ( TablePrice t ) => t );

        var result = await _service.CreateFromDtoAsync(dto);

        Assert.NotNull ( result );
        Assert.Equal ( 2, result.Items.Count );
        Assert.Equal ( "Tabela verão", result.Name );
    }

    [Fact]
    public async Task Create_WithoutItems_ShouldThrowValidationException ( )
    {
        var dto = new CreateTablePriceDto
        {
            Name = "Tabela vazia",
            Description = "Sem itens",
            Items = []
        };

        await Assert.ThrowsAsync<CustomValidationException> ( ( ) =>
            _service.CreateFromDtoAsync ( dto )
        );
    }

    [Fact]
    public async Task Create_WithDuplicateWorkTypes_ShouldThrowBusinessException ( )
    {
        var dto = new CreateTablePriceDto
        {
            Name = "Tabela duplicada",
            Description = "Contém serviços repetidos",
            Items = new List<TablePriceItemInputDto>
            {
                new() { WorkTypeId = 1, Price = 100 },
                new() { WorkTypeId = 1, Price = 120 } // duplicado
            }
        };

        await Assert.ThrowsAsync<BusinessRuleException> ( ( ) =>
            _service.CreateFromDtoAsync ( dto )
        );
    }
    [Fact]
    public async Task Get_Price_By_Client_And_WorkType_Should_Return_Correct_Price ( )
    {
        // Arrange
        _clientRepoMock.Setup ( r => r.GetEntityWithSpec ( It.IsAny<ISpecification<Client>> ( ) ) )
            .ReturnsAsync ( new Client
            {
                ClientId = 1,
                ClientName = "Clínica Sorriso Ideal",
                TablePrice = new TablePrice
                {
                    Name = "Tabela Teste",
                    Description = "Descrição",
                    Status = true,
                    Items = new List<TablePriceItem>
        {
        new()
        {
            WorkTypeId = 2,
            Price = 250,
            WorkType = new WorkType
            {
                Id = 2,
                Name = "PSI",
                WorkSectionId = 1,
                WorkSection = new WorkSection { Id = 1, Name = "Implantes" }
            }
        }
        }
                }

            } );

        // Act
        var result = await _service.GetItemPriceByClientAndWorkTypeAsync(1, 2);

        // Assert
        Assert.NotNull ( result );
        Assert.Equal ( 2, result.WorkTypeId );
        Assert.Equal ( "PSI", result.WorkTypeName );
        Assert.Equal ( 250, result.Price );
    }



    [Fact]
    public async Task Create_WithInvalidWorkType_ShouldThrowNotFoundException ( )
    {
        var dto = new CreateTablePriceDto
        {
            Name = "Tabela com tipo inválido",
            Description = "Tipo de trabalho não existe",
            Items = new List<TablePriceItemInputDto>
            {
                new() { WorkTypeId = 999, Price = 80 }
            }
        };

        _workTypeRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<WorkType>> ( ) ) )
            .ReturnsAsync ( new List<WorkType> ( ) );

        await Assert.ThrowsAsync<NotFoundException> ( ( ) =>
            _service.CreateFromDtoAsync ( dto )
        );
    }
}
