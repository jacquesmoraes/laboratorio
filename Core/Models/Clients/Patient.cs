using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Clients
{
    public class Patient
    {
        public required string Name { get; set; }
        public string? Notes { get; set; }
    }
}
