namespace Core.Models.Identity
{
    public interface IApplicationRole
    {
        string Id { get; }
        string? Name { get; }
        DateTime CreatedAt { get; }
    }
}