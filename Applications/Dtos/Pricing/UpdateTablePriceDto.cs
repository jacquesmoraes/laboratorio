namespace Applications.Dtos.Pricing
{
    public class UpdateTablePriceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Status { get; set; }
    public List<UpdateTablePriceItemDto> Items { get; set; } = [];
}

}
