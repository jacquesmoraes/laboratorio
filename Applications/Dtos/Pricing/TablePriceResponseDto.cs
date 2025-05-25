using Applications.Dtos.Clients;

namespace Applications.Dtos.Pricing
{
    public class TablePriceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }

        public List<TablePriceItemShortDto> Items { get; set; } = [];
        public List<ClientResponseForTablePriceDto> Clients { get; set; } = [];
    }

}
