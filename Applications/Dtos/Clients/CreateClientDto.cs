﻿namespace Applications.Dtos.Clients
{
    public class CreateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? Cro { get; set; }
        public string? Cnpj { get; set; }
        public string? Cpf { get; set; }
        public string? PhoneNumber { get; set; }

        [EnumDataType ( typeof ( BillingMode ), ErrorMessage = "billing mode invalid." )]
        public BillingMode BillingMode { get; set; }
        public ClientAddressRecord Address { get; set; } = new ( );

        public int? TablePriceId { get; set; }
    }


}
