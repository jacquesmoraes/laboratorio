using Applications.Dtos.Pricing;
using Applications.Services;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Pricing;
using Core.Specifications;
using Moq;

namespace Tests.Unit.Applications
{
    public class TablePriceServiceTests
    {
        private readonly Mock<IGenericRepository<TablePrice>> _tableRepoMock = new();
        private readonly Mock<IGenericRepository<TablePriceItem>> _itemRepoMock = new();

        private readonly TablePriceService _service;

        public TablePriceServiceTests ( )
        {
            _service = new TablePriceService ( _tableRepoMock.Object, _itemRepoMock.Object );
        }

        [Fact]
        public async Task Create_ValidTablePriceWithItems_ShouldSucceed ( )
        {
            // Arrange
            var dto = new CreateTablePriceDto
            {
                Name = "Tabela verão",
                Description = "Descontos especiais",
                Items = new List<TablePriceItemDtoForTablePrice>
            {
                new() { TablePriceItemId = 1 },
                new() { TablePriceItemId = 2 }
            }
            };

            var fakeItems = new List<TablePriceItem>
        {
            new() { TablePriceItemId = 1, ItemName = "A", Price = 10 },
            new() { TablePriceItemId = 2, ItemName = "B", Price = 20 }
        };

            _itemRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<TablePriceItem>> ( ) ) )
                .ReturnsAsync ( fakeItems );

            _tableRepoMock.Setup ( r => r.CreateAsync ( It.IsAny<TablePrice> ( ) ) )
                .ReturnsAsync ( ( TablePrice t ) => t );

            // Act
            var result = await _service.CreateFromDtoAsync(dto);

            // Assert
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

            await Assert.ThrowsAsync<CustomValidationException> ( ( ) => _service.CreateFromDtoAsync ( dto ) );
        }

        [Fact]
        public async Task Create_WithDuplicateItems_ShouldThrowBusinessException ( )
        {
            var dto = new CreateTablePriceDto
            {
                Name = "Tabela duplicada",
                Description = "Contém itens duplicados",
                Items = new List<TablePriceItemDtoForTablePrice>
            {
                new() { TablePriceItemId = 1 },
                new() { TablePriceItemId = 1 }
            }
            };

            await Assert.ThrowsAsync<BusinessRuleException> ( ( ) => _service.CreateFromDtoAsync ( dto ) );
        }

        [Fact]
        public async Task Create_WithInvalidItemId_ShouldThrowNotFoundException ( )
        {
            var dto = new CreateTablePriceDto
            {
                Name = "Tabela com item inválido",
                Description = "Item não existe",
                Items = new List<TablePriceItemDtoForTablePrice>
            {
                new() { TablePriceItemId = 999 }
            }
            };

            _itemRepoMock.Setup ( r => r.GetAllAsync ( It.IsAny<ISpecification<TablePriceItem>> ( ) ) )
                .ReturnsAsync ( new List<TablePriceItem> ( ) ); // nenhum encontrado

            await Assert.ThrowsAsync<NotFoundException> ( ( ) => _service.CreateFromDtoAsync ( dto ) );
        }
    }
}