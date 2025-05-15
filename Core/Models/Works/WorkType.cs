namespace Core.Models.Works
{
    public class WorkType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public int WorkSectionId { get; set; }
        public WorkSection WorkSection { get; set; } = null!;
    }
}
