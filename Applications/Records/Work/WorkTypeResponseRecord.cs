﻿namespace Applications.Records.Work
{
    public record WorkTypeResponseRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public string WorkSectionName { get; init; } = string.Empty;
    }

}
