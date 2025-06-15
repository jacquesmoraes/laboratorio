namespace Applications.Dtos.Pricing
{
    public class UpdateTablePriceItemDto
    {

        public int Id { get; set; }
        public required string ItemName { get; set; }
        public required decimal Price { get; set; }
    }
}
