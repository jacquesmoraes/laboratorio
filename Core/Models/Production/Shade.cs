namespace Core.Models.Production
{
    public class Shade
    {
        public int Id { get; set; }
        public string? color { get; set; }

        public int ScaleId { get; set; }
        public Scale? Scale { get; set; } 
    }
}