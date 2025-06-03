using System.ComponentModel.DataAnnotations;

namespace Core.Models.Production
{
    public class Shade
    {
        public int Id { get; set; }
        [Required]
        public string color { get; set; } = string.Empty;

        public int ScaleId { get; set; }
        public Scale? Scale { get; set; } 
    }
}