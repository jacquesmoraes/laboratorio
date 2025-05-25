namespace Applications.Dtos.ServiceOrder
{
    public class CreateServiceOrderDto
    {
        public int ClientId { get; set; }

        public DateTime DateIn { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public int FirstSectorId { get; set; }

        public List<CreateWorkDto> Works { get; set; } = [];

    }
}
