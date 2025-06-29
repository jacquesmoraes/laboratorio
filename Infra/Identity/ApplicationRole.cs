namespace Infra.Identity
{
    public class ApplicationRole : IdentityRole, IApplicationRole
    {

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}