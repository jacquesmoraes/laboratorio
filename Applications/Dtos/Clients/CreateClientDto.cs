namespace Applications.Dtos.Clients
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "CRO deve ter no máximo 20 caracteres")]
        public string? Cro { get; set; }

        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$", ErrorMessage = "CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX")]
        public string? Cnpj { get; set; }

        [StringLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF deve estar no formato XXX.XXX.XXX-XX")]
        public string? Cpf { get; set; }

        [Phone(ErrorMessage = "Número de telefone deve ter um formato válido")]
        public string? PhoneNumber { get; set; }

        [EnumDataType(typeof(BillingMode), ErrorMessage = "Modo de faturamento inválido")]
        public BillingMode BillingMode { get; set; }

        [Required(ErrorMessage = "Endereço é obrigatório")]
        public ClientAddressRecord Address { get; set; } = new();

        [Range(1, int.MaxValue, ErrorMessage = "ID da tabela de preços deve ser maior que zero")]
        public int? TablePriceId { get; set; }
    }


}
