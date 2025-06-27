namespace Core.Models.Identity
{

    public interface IApplicationUser
    {
        string Id { get; }
        string? Email { get; }
        string DisplayName { get; }
        bool IsActive { get; }
        bool IsFirstLogin { get; }
        int? ClientId { get; }
        DateTime CreatedAt { get; }
        DateTime? LastLoginAt { get; }
        string? AccessCode { get; }
        DateTime? AccessCodeExpiresAt { get; }
    }
}

