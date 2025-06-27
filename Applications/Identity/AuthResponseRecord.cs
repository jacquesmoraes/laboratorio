namespace Applications.Identity
{
    public record AuthResponseRecord
    {
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public UserInfoRecord User { get; init; }

        public AuthResponseRecord ( string token, UserInfoRecord user )
        {
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddHours(6);;
            User = user;
        }
    }
}