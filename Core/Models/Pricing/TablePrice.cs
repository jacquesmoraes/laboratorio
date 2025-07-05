using Core.Models.Clients;

namespace Core.Models.Pricing
{
    public class TablePrice
    {
        public int Id { get; set; }
        public required string? Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
            
        public List<TablePriceItem> Items { get; set; } = [];
        public List<Client> Clients { get; set; } = [];
    }
}
