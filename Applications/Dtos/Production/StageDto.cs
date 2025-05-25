namespace Applications.Dtos.Production
{
    public class StageDto
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; } = string.Empty; // de Sector.Name
        public DateTime DateIn { get; set; }
        public DateTime? DateOut { get; set; }
    }

}
