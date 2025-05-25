using Core.Models.Works;

namespace Core.Models.Pricing
{
    public class TablePriceItem
    {
        public int Id { get; set; }
        public int TablePriceId { get; set; }
        public TablePrice? TablePrice { get; set; }
        public int WorkTypeId { get; set; }
        public WorkType? WorkType { get; set; }
        public decimal Price { get; set; }
    }
}