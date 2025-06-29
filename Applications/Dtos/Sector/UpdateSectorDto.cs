namespace Applications.Dtos.Sector
{
    public class UpdateSectorDto
    {
        [Required]
        public int SectorId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
