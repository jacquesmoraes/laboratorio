using System.ComponentModel.DataAnnotations;

namespace Core.Models.Production
{
    public class Scale
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public  List<Shade> Colors { get; set; } = [];
    }
}
