namespace Applications.Dtos.Clients
{
    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Complement { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
