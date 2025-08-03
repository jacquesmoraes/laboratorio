namespace Applications.Identity
{
    public record ClientUserListRecord
    {
        public string UserId { get; init; } = string.Empty;
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public bool HasValidAccessCode { get; init; }
    }
}