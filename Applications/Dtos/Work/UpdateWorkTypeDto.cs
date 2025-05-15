namespace Applications.Dtos.Work
{
    public class UpdateWorkTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int WorkSectionId { get; set; }
    }
}
