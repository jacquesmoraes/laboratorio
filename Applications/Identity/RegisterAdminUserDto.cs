namespace Applications.Identity
{
     public class RegisterAdminUserDto
    {
        [Required(ErrorMessage = "Nome de exibição é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "Senha e confirmação devem ser iguais")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}