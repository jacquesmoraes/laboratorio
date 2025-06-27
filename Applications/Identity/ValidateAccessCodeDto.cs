namespace Applications.Identity
{
    public class ValidateAccessCodeDto
    {
        public string Email { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
    }
}
