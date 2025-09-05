namespace Infra.Identity
{
    public class UserLoginHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime LoginAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}

