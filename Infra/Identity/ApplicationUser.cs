namespace Infra.Identity
{
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFirstLogin { get; set; } = true;
        public int? ClientId { get; set; }
        public string? AccessCode { get; set; }
        public DateTime? AccessCodeExpiresAt { get; set; }
    }
}