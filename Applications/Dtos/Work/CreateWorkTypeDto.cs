namespace Applications.Dtos.Work
{
    public class CreateWorkTypeDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int WorkSectionId { get; set; }
    }
}
