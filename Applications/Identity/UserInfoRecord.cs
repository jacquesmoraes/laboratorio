namespace Applications.Identity
{
     public record UserInfoRecord
    {
        public string UserId { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public int? ClientId { get; init; }
        public bool IsFirstLogin { get; init; }
        public string AccessCode { get; init; } = string.Empty;
    }
}