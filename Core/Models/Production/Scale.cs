namespace Core.Models.Production
{
    public class Scale
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<Shade> Colors { get; set; } = [];
    }
}
