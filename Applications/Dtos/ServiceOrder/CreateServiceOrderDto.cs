namespace Applications.Dtos.ServiceOrder
{
    public class CreateServiceOrderDto
    {
        public int ClientId { get; set; }

        public DateTime DateIn { get; set; }

        [Required ( ErrorMessage = "Nome do paciente é obrigatório" )]
        public string PatientName { get; set; } = string.Empty;

        [Required ( ErrorMessage = "ID do primeiro setor é obrigatório" )]
        public int FirstSectorId { get; set; }

        [Required ( ErrorMessage = "Pelo menos um trabalho deve ser adicionado" )]
        public List<CreateWorkDto> Works { get; set; } = [];

        public bool IsRepeat { get; set; }
        public RepeatResponsible? RepeatResponsible { get; set; }

    }
}
