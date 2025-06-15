namespace Applications.Records.Pricing
{
    public record TablePriceItemsResponseRecord
    {
        public int Id { get; init; }                  // ← pode mudar para TablePriceItemId por clareza
        public string ItemName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string? TablePriceName { get; init; } // ← precisa do Include e do MapFrom
    }

}
