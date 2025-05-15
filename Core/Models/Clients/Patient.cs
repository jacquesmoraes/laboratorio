using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Clients
{
    public class Patient
    {
        public int PatientId { get; set; }
        public required string Name { get; set; }
        public string? Notes { get; set; }

        public int ClientId { get; set; }

        [ForeignKey ( nameof ( ClientId ) )]
        public required Client Client { get; set; }
    }
}
