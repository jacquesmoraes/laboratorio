namespace Applications.Identity
{
    public class ResetPasswordDto
    {
        [Required]
        public required string Token { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        
        public required string NewPassword { get; set; }
    }
}