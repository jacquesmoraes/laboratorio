using System.ComponentModel.DataAnnotations;

namespace Applications.Dtos.Sector
{
    public class CreateSectorDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }

}
