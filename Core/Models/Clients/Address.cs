namespace Core.Models.Clients
{
    public class Address
    {
        public string? Street { get; set; }
        public int? Number { get; set; }
        public string? Complement { get; set; }

        public string? Neighborhood { get; set; }
        public string? City { get; set; }
    }
}
