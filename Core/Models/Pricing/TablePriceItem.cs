namespace Core.Models.Pricing
{
    public class TablePriceItem
    {
        public int TablePriceItemId { get; set; }

        public required string ItemName { get; set; }
        public required decimal Price { get; set; }
        public int? TablePriceId { get; set; }
        public TablePrice? TablePrice { get; set; }

    }
}