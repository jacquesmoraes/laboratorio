namespace Applications.Dtos.Pricing
{
    public class TablePriceItemShortDto
    {
        public int Id { get; set; }
        public string WorkTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}