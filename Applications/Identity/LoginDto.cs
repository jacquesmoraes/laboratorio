namespace Applications.Identity
{
     public class LoginDto
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Senha deve ter pelo menos 1 caractere")]
        public string Password { get; set; } = string.Empty;
    }
}