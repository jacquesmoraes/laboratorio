namespace Applications.Dtos.Work
{
    public class WorkTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string WorkSectionName { get; set; } = string.Empty;
    }
}
