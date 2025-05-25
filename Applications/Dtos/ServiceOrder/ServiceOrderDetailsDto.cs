using Applications.Dtos.Clients;
using Applications.Dtos.Production;

namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderDetailsDto
    {
        public int ServiceOrderId { get; set; }
        public int OrderNumber { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public DateTime? DateOutFinal { get; set; }
        public decimal OrderTotal { get; set; }
        public string? CurrentSectorName { get; set; }
        public ClientResponseForOrderServiceDto Client { get; set; } = new ( );
        public List<WorkDto> Works { get; set; } = [];
        public List<StageDto> Stages { get; set; } = [];
    }

}
