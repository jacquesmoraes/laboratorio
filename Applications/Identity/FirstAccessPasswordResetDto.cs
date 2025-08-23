namespace Applications.Identity
{
     public class FirstAccessPasswordResetDto
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Código de acesso é obrigatório")]
       
        public string AccessCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "Senha e confirmação devem ser iguais")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}