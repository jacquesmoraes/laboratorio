using Applications.Records.Clients;
using Applications.Records.Pricing;

namespace Applications.Projections.Pricing
{
    public record TablePriceResponseProjection
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool Status { get; init; }

        public List<TablePriceItemShortRecord> Items { get; init; } = [];
        public List<ClientResponseForTablePriceRecord> Clients { get; init; } = [];
    }

}
