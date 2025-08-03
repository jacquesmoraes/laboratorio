namespace Applications.Identity
{
    public record ClientUserDetailsRecord
    {
        public string UserId { get; init; } = string.Empty; 
        public int ClientId { get; init; }
        public string ClientName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? LastLoginAt { get; init; }

        public bool IsFirstLogin { get; init; } 

        public string? AccessCode { get; init; }
        public DateTime? AccessCodeExpiresAt { get; init; }

        public bool IsAccessCodeValid { get; init; }

        public List<LoginHistoryRecord>? LoginHistory { get; init; }
    }

    public record LoginHistoryRecord
    {
        public DateTime LoginAt { get; init; }
        public string IpAddress { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
    }
}
